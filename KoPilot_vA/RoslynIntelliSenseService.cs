using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using System.Reflection;



namespace KoPilot_vA
{
    /// <summary>
    /// Roslyn-backed service providing syntax highlighting tokens, completion items,
    /// and diagnostics for a single C# document. One instance is kept per open editor tab.
    /// </summary>
    public class RoslynIntelliSenseService : IDisposable
    {
        private static readonly MefHostServices _hostServices = CreateHostServices();

        private static readonly IReadOnlyList<MetadataReference> _defaultReferences =
            BuildDefaultReferences();

        private readonly AdhocWorkspace _workspace;
        private ProjectId _projectId;
        private DocumentId _documentId;
        private readonly Dictionary<string, DocumentId> _siblingDocs = new(StringComparer.OrdinalIgnoreCase);
        private bool _disposed;

        public RoslynIntelliSenseService()
        {
            _workspace = new AdhocWorkspace(_hostServices);
            InitialiseProject();
        }

        private static MefHostServices CreateHostServices()
        {
            var assemblies = MefHostServices.DefaultAssemblies.ToList();
            TryAddAssembly(assemblies, "Microsoft.CodeAnalysis.Features");
            TryAddAssembly(assemblies, "Microsoft.CodeAnalysis.CSharp.Features");
            return MefHostServices.Create(assemblies);
        }

        private static void TryAddAssembly(List<Assembly> list, string name)
        {
            try
            {
                var asm = Assembly.Load(name);
                if (!list.Contains(asm))
                    list.Add(asm);
            }
            catch { }
        }

        // ?? Project / document management ??????????????????????????????????????

        private void InitialiseProject()
        {
            var compilationOptions = new CSharpCompilationOptions(OutputKind.ConsoleApplication)
                .WithNullableContextOptions(NullableContextOptions.Enable);

            var parseOptions = new CSharpParseOptions(LanguageVersion.CSharp12);

            var projectInfo = ProjectInfo.Create(
                ProjectId.CreateNewId(),
                VersionStamp.Create(),
                "IntelliSenseProject",
                "IntelliSenseProject",
                LanguageNames.CSharp,
                compilationOptions: compilationOptions,
                parseOptions: parseOptions,
                metadataReferences: _defaultReferences);

            var project = _workspace.AddProject(projectInfo);
            _projectId = project.Id;

            var docInfo = DocumentInfo.Create(
                DocumentId.CreateNewId(_projectId),
                "Document.cs",
                sourceCodeKind: SourceCodeKind.Regular,
                loader: TextLoader.From(TextAndVersion.Create(
                    SourceText.From(string.Empty), VersionStamp.Create())));

            _workspace.AddDocument(docInfo);
            _documentId = _workspace.CurrentSolution
                .GetProject(_projectId)!.DocumentIds[0];
        }

        /// <summary>Updates the active document text without replacing the document identity.</summary>
        public void UpdateText(string sourceText)
        {
            var solution = _workspace.CurrentSolution
                .WithDocumentText(_documentId,
                    SourceText.From(sourceText));
            _workspace.TryApplyChanges(solution);
        }

        /// <summary>
        /// Registers or updates a sibling .cs file in the workspace so the semantic
        /// model resolves types and members declared in other project files.
        /// </summary>
        public void AddOrUpdateSiblingDocument(string absolutePath, string sourceText)
        {
            if (_siblingDocs.TryGetValue(absolutePath, out var existingId))
            {
                var solution = _workspace.CurrentSolution
                    .WithDocumentText(existingId, SourceText.From(sourceText));
                _workspace.TryApplyChanges(solution);
            }
            else
            {
                var docInfo = DocumentInfo.Create(
                    DocumentId.CreateNewId(_projectId),
                    Path.GetFileName(absolutePath),
                    sourceCodeKind: SourceCodeKind.Regular,
                    loader: TextLoader.From(TextAndVersion.Create(
                        SourceText.From(sourceText), VersionStamp.Create())),
                    filePath: absolutePath);

                var doc = _workspace.AddDocument(docInfo);
                _siblingDocs[absolutePath] = doc.Id;
            }
        }

        /// <summary>Removes a sibling document from the workspace.</summary>
        public void RemoveSiblingDocument(string absolutePath)
        {
            if (_siblingDocs.TryGetValue(absolutePath, out var id))
            {
                var solution = _workspace.CurrentSolution.RemoveDocument(id);
                _workspace.TryApplyChanges(solution);
                _siblingDocs.Remove(absolutePath);
            }
        }

        private Document CurrentDocument =>
            _workspace.CurrentSolution.GetDocument(_documentId)!;

        // ?? Syntax highlighting ????????????????????????????????????????????????

        /// <summary>
        /// Returns classified spans for the entire document.
        /// Each span carries a classification name (keyword, string-literal, comment, etc.)
        /// and maps directly to a character range in the source text.
        /// </summary>
        public async Task<IReadOnlyList<ClassifiedSpan>> GetClassifiedSpansAsync(
            CancellationToken ct = default)
        {
            var doc = CurrentDocument;
            var text = await doc.GetTextAsync(ct).ConfigureAwait(false);
            var spans = await Classifier.GetClassifiedSpansAsync(
                doc, new TextSpan(0, text.Length), ct).ConfigureAwait(false);
            return spans.ToList();
        }

        // ?? Completion ?????????????????????????????????????????????????????????

        /// <summary>
        /// Returns completion candidates at the given caret offset.
        /// Uses <see cref="CompletionService"/> when available (requires CSharp.Features),
        /// otherwise falls back to semantic symbol lookup.
        /// </summary>
        public async Task<IReadOnlyList<CompletionCandidate>> GetCompletionsAsync(
            int caretPosition, char triggerChar = '\0', CancellationToken ct = default)
        {
            var doc = CurrentDocument;

            // Build the correct trigger so Roslyn knows whether this is a member-access
            // invocation (dot) or a regular insertion (letters/digits/Ctrl+Space).
            var trigger = triggerChar == '.'
                ? CompletionTrigger.CreateInsertionTrigger('.')
                : CompletionTrigger.Invoke;

            // Primary path: full IDE-quality completions via CompletionService
            var completionService = CompletionService.GetService(doc);
            if (completionService != null)
            {
                try
                {
                    var list = await completionService
                        .GetCompletionsAsync(doc, caretPosition, trigger, cancellationToken: ct)
                        .ConfigureAwait(false);

                    if (list != null && list.ItemsList.Count > 0)
                    {
                        return list.ItemsList
                            .Select(item => new CompletionCandidate(
                                item.DisplayText,
                                item.Tags.FirstOrDefault() ?? string.Empty))
                            .OrderBy(c => c.DisplayText)
                            .ToList();
                    }
                }
                catch (OperationCanceledException) { throw; }
                catch { /* fall through to semantic fallback */ }
            }

            // Fallback: semantic symbol lookup
            return await GetCompletionsFallbackAsync(doc, caretPosition, ct).ConfigureAwait(false);
        }

        private static async Task<IReadOnlyList<CompletionCandidate>> GetCompletionsFallbackAsync(
            Document doc, int caretPosition, CancellationToken ct)
        {
            var syntaxRoot = await doc.GetSyntaxRootAsync(ct).ConfigureAwait(false);
            var semanticModel = await doc.GetSemanticModelAsync(ct).ConfigureAwait(false);
            if (syntaxRoot == null || semanticModel == null)
                return Array.Empty<CompletionCandidate>();

            var safePos = Math.Max(0, Math.Min(caretPosition, syntaxRoot.FullSpan.End));
            var token = syntaxRoot.FindToken(Math.Max(0, safePos - 1));

            var memberAccess = token.Parent?
                .AncestorsAndSelf()
                .OfType<MemberAccessExpressionSyntax>()
                .FirstOrDefault();

            if (memberAccess != null)
            {
                var exprType = semanticModel.GetTypeInfo(memberAccess.Expression, ct).Type;
                if (exprType != null)
                    return GetMembersOf(exprType);
            }

            var scope = semanticModel.LookupSymbols(safePos);
            return scope
                .Select(s => new CompletionCandidate(s.Name, SymbolKindToTag(s.Kind)))
                .OrderBy(c => c.DisplayText)
                .ToList();
        }

        private static IReadOnlyList<CompletionCandidate> GetMembersOf(ITypeSymbol type)
        {
            var results = new List<CompletionCandidate>();
            var seen = new HashSet<string>();

            ITypeSymbol? current = type;
            while (current != null)
            {
                foreach (var member in current.GetMembers())
                {
                    if (member.IsImplicitlyDeclared) continue;
                    var acc = member.DeclaredAccessibility;
                    if (acc != Microsoft.CodeAnalysis.Accessibility.Public &&
                        acc != Microsoft.CodeAnalysis.Accessibility.Protected) continue;
                    // Skip accessor methods (get_Foo, set_Foo, add_Foo, etc.)
                    if (member is IMethodSymbol ms &&
                        ms.MethodKind != MethodKind.Ordinary &&
                        ms.MethodKind != MethodKind.Constructor) continue;

                    if (seen.Add(member.Name))
                        results.Add(new CompletionCandidate(member.Name, SymbolKindToTag(member.Kind)));
                }
                current = current.BaseType;
            }

            return results.OrderBy(r => r.DisplayText).ToList();
        }

        private static string SymbolKindToTag(SymbolKind kind) => kind switch
        {
            SymbolKind.NamedType  => "Class",
            SymbolKind.Method     => "Method",
            SymbolKind.Property   => "Property",
            SymbolKind.Field      => "Field",
            SymbolKind.Local      => "Local",
            SymbolKind.Parameter  => "Parameter",
            SymbolKind.Namespace  => "Namespace",
            _                     => kind.ToString()
        };


        // ?? Diagnostics ????????????????????????????????????????????????????????

        /// <summary>
        /// Returns Roslyn compiler diagnostics (errors and warnings) for the current document.
        /// </summary>
        public async Task<IReadOnlyList<Diagnostic>> GetDiagnosticsAsync(
            CancellationToken ct = default)
        {
            var doc = CurrentDocument;
            var semanticModel = await doc.GetSemanticModelAsync(ct).ConfigureAwait(false);
            if (semanticModel == null) return Array.Empty<Diagnostic>();
            return semanticModel.GetDiagnostics(cancellationToken: ct);
        }

        // ?? Quick info (hover) ?????????????????????????????????????????????????

        /// <summary>
        /// Returns a display string for the symbol at the given caret offset, or null.
        /// </summary>
        public async Task<string?> GetQuickInfoAsync(int caretPosition, CancellationToken ct = default)
        {
            var doc = CurrentDocument;
            var semanticModel = await doc.GetSemanticModelAsync(ct).ConfigureAwait(false);
            var syntaxRoot = await doc.GetSyntaxRootAsync(ct).ConfigureAwait(false);
            if (semanticModel == null || syntaxRoot == null) return null;

            var node = syntaxRoot.FindToken(caretPosition).Parent;
            if (node == null) return null;

            var symbolInfo = semanticModel.GetSymbolInfo(node, ct);
            var symbol = symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault();
            return symbol?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        }

        // ?? Default references ?????????????????????????????????????????????????

        private static IReadOnlyList<MetadataReference> BuildDefaultReferences()
        {
            var refs = new List<MetadataReference>();

            // Core runtime assemblies from the currently running process
            string runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            string[] coreAssemblies =
            {
                "System.Runtime.dll",
                "System.Console.dll",
                "System.Collections.dll",
                "System.Linq.dll",
                "System.Threading.dll",
                "System.Threading.Tasks.dll",
                "System.IO.dll",
                "System.Net.Http.dll",
                "System.Text.Json.dll",
                "System.Private.CoreLib.dll",
                "netstandard.dll"
            };

            foreach (var asm in coreAssemblies)
            {
                var path = Path.Combine(runtimeDir, asm);
                if (File.Exists(path))
                    refs.Add(MetadataReference.CreateFromFile(path));
            }

            // Also add the mscorlib / object assembly itself
            refs.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            refs.Add(MetadataReference.CreateFromFile(typeof(Console).Assembly.Location));
            refs.Add(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location));
            refs.Add(MetadataReference.CreateFromFile(typeof(System.Text.Json.JsonSerializer).Assembly.Location));

            return refs;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _workspace.Dispose();
                _disposed = true;
            }
        }
    }

    /// <summary>A single completion candidate returned by the service.</summary>
    public sealed class CompletionCandidate
    {
        public string DisplayText { get; }
        /// <summary>Roslyn WellKnownTag string, e.g. "Method", "Class", "Property".</summary>
        public string Kind { get; }

        public CompletionCandidate(string displayText, string kind)
        {
            DisplayText = displayText;
            Kind = kind;
        }
    }
}

