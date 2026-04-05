using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Classification;



namespace KoPilot_vA
{
    public class EditorTabPage : TabPage
    {
        public string FilePath { get; set; }
        public bool IsModified { get; private set; }
        public RichTextBox Editor { get; }
        public RoslynIntelliSenseService? RoslynService => _roslyn;
        private readonly LineNumberGutter _gutter;
        private readonly System.Windows.Forms.Timer _highlightTimer;
        private readonly System.Windows.Forms.Timer _completionTimer;
        private readonly RoslynIntelliSenseService? _roslyn;
        private readonly CompletionPopup _completionPopup;
        private readonly ContextMenuStrip _editorContextMenu;
        private string _originalContent = string.Empty;
        private CancellationTokenSource _highlightCts = new();
        private CancellationTokenSource _completionCts = new();
        private char _lastTypedChar = '\0';

        // Classification type to editor colour mapping
        private static readonly Dictionary<string, Color> _classificationColors = new()
        {
            [ClassificationTypeNames.Keyword]                    = Color.FromArgb(86,  156, 214),
            [ClassificationTypeNames.ControlKeyword]             = Color.FromArgb(216, 160, 223),
            [ClassificationTypeNames.ClassName]                  = Color.FromArgb(78,  201, 176),
            [ClassificationTypeNames.RecordClassName]            = Color.FromArgb(78,  201, 176),
            [ClassificationTypeNames.StructName]                 = Color.FromArgb(78,  201, 176),
            [ClassificationTypeNames.RecordStructName]           = Color.FromArgb(78,  201, 176),
            [ClassificationTypeNames.InterfaceName]              = Color.FromArgb(184, 215, 163),
            [ClassificationTypeNames.EnumName]                   = Color.FromArgb(184, 215, 163),
            [ClassificationTypeNames.EnumMemberName]             = Color.FromArgb(181, 206, 168),
            [ClassificationTypeNames.DelegateName]               = Color.FromArgb(78,  201, 176),
            [ClassificationTypeNames.TypeParameterName]          = Color.FromArgb(184, 215, 163),
            [ClassificationTypeNames.MethodName]                 = Color.FromArgb(220, 220, 170),
            [ClassificationTypeNames.ExtensionMethodName]        = Color.FromArgb(220, 220, 170),
            [ClassificationTypeNames.PropertyName]               = Color.FromArgb(156, 220, 254),
            [ClassificationTypeNames.FieldName]                  = Color.FromArgb(156, 220, 254),
            [ClassificationTypeNames.ConstantName]               = Color.FromArgb(181, 206, 168),
            [ClassificationTypeNames.EventName]                  = Color.FromArgb(156, 220, 254),
            [ClassificationTypeNames.LocalName]                  = Color.FromArgb(156, 220, 254),
            [ClassificationTypeNames.ParameterName]              = Color.FromArgb(156, 220, 254),
            [ClassificationTypeNames.LabelName]                  = Color.FromArgb(220, 220, 220),
            [ClassificationTypeNames.NamespaceName]              = Color.FromArgb(220, 220, 220),
            [ClassificationTypeNames.StringLiteral]              = Color.FromArgb(214, 157, 133),
            [ClassificationTypeNames.VerbatimStringLiteral]      = Color.FromArgb(214, 157, 133),
            [ClassificationTypeNames.StringEscapeCharacter]      = Color.FromArgb(255, 214, 143),
            [ClassificationTypeNames.NumericLiteral]             = Color.FromArgb(181, 206, 168),
            [ClassificationTypeNames.Comment]                    = Color.FromArgb(87,  166,  74),
            [ClassificationTypeNames.XmlDocCommentText]          = Color.FromArgb(87,  166,  74),
            [ClassificationTypeNames.XmlDocCommentDelimiter]     = Color.FromArgb(128, 128, 128),
            [ClassificationTypeNames.XmlDocCommentName]          = Color.FromArgb(128, 128, 128),
            [ClassificationTypeNames.XmlDocCommentAttributeName] = Color.FromArgb(128, 128, 128),
            [ClassificationTypeNames.ExcludedCode]               = Color.FromArgb(128, 128, 128),
            [ClassificationTypeNames.PreprocessorKeyword]        = Color.FromArgb(155, 155, 155),
            [ClassificationTypeNames.PreprocessorText]           = Color.FromArgb(155, 155, 155),
            [ClassificationTypeNames.Operator]                   = Color.FromArgb(180, 180, 180),
            [ClassificationTypeNames.OperatorOverloaded]         = Color.FromArgb(220, 220, 170),
            [ClassificationTypeNames.StaticSymbol]               = Color.FromArgb(220, 220, 170),
        };

        private static readonly Color _defaultColor = Color.FromArgb(220, 220, 220);

        public EditorTabPage(string filePath) : base(Path.GetFileName(filePath))
        {
            FilePath = filePath;
            Tag = filePath;

            Editor = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 11f),
                AcceptsTab = true,
                WordWrap = false,
                DetectUrls = false,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.FromArgb(220, 220, 220),
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Both,
                HideSelection = false
            };

            var isCSharp = IsCSharpFile(filePath);

            // Roslyn service only for C# files
            if (isCSharp)
            {
                _roslyn = new RoslynIntelliSenseService();
            }

            // Completion popup (shared owner = this tab, never steals focus)
            _completionPopup = new CompletionPopup();
            _completionPopup.ItemCommitted += CompletionPopup_ItemCommitted;

            Editor.TextChanged   += Editor_TextChanged;
            Editor.KeyDown       += Editor_KeyDown;
            Editor.KeyPress      += Editor_KeyPress;
            Editor.MouseDown     += Editor_MouseDown;
            Editor.LostFocus     += (_, _) => _completionPopup.Hide();
            Editor.SelectionChanged += (_, _) => _gutter.Invalidate();

            // Highlight debounce: 350 ms after last keystroke
            _highlightTimer = new System.Windows.Forms.Timer { Interval = 350 };
            _highlightTimer.Tick += async (_, _) =>
            {
                _highlightTimer.Stop();
                try { await ApplySyntaxHighlightingAsync(); }
                catch { /* logged via global handler if critical */ }
            };

            // Completion debounce: 400 ms after last keystroke
            _completionTimer = new System.Windows.Forms.Timer { Interval = 400 };
            _completionTimer.Tick += async (_, _) =>
            {
                _completionTimer.Stop();
                try { await ShowCompletionsAsync(forceRefetch: true); }
                catch { /* logged via global handler if critical */ }
            };

            _gutter = new LineNumberGutter(Editor) { Dock = DockStyle.Left };

            var editorPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30)
            };
            editorPanel.Controls.Add(Editor);
            editorPanel.Controls.Add(_gutter);

            Controls.Add(editorPanel);

            // Editor context menu
            _editorContextMenu = BuildEditorContextMenu();
            Editor.ContextMenuStrip = _editorContextMenu;
        }

        private ContextMenuStrip BuildEditorContextMenu()
        {
            var menu = new ContextMenuStrip
            {
                Renderer = new EditorContextMenuRenderer(),
                BackColor = Color.FromArgb(37, 37, 38),
                ForeColor = Color.FromArgb(220, 220, 220),
                ShowImageMargin = false
            };

            var cut    = new ToolStripMenuItem("Cut",    null, (_, _) => Editor.Cut())    { ShortcutKeyDisplayString = "Ctrl+X" };
            var copy   = new ToolStripMenuItem("Copy",   null, (_, _) => Editor.Copy())   { ShortcutKeyDisplayString = "Ctrl+C" };
            var paste  = new ToolStripMenuItem("Paste",  null, (_, _) => Editor.Paste())  { ShortcutKeyDisplayString = "Ctrl+V" };
            var delete = new ToolStripMenuItem("Delete", null, (_, _) => { if (Editor.SelectionLength > 0) Editor.SelectedText = string.Empty; });

            var selectAll = new ToolStripMenuItem("Select All", null, (_, _) => Editor.SelectAll()) { ShortcutKeyDisplayString = "Ctrl+A" };

            var undo = new ToolStripMenuItem("Undo", null, (_, _) => { if (Editor.CanUndo) Editor.Undo(); }) { ShortcutKeyDisplayString = "Ctrl+Z" };
            var redo = new ToolStripMenuItem("Redo", null, (_, _) => { if (Editor.CanRedo) Editor.Redo(); }) { ShortcutKeyDisplayString = "Ctrl+Y" };

            var toggleComment      = new ToolStripMenuItem("Toggle Comment",          null, (_, _) => ToggleLineComment())        { ShortcutKeyDisplayString = "Ctrl+/" };
            var duplicateLine      = new ToolStripMenuItem("Duplicate Line",          null, (_, _) => DuplicateCurrentLine())     { ShortcutKeyDisplayString = "Ctrl+D" };
            var fixIndentation     = new ToolStripMenuItem("Fix Indentation",         null, (_, _) => FixIndentation());
            var trimWhitespace     = new ToolStripMenuItem("Trim Trailing Whitespace", null, (_, _) => TrimTrailingWhitespace());

            menu.Items.AddRange([
                undo, redo,
                new ToolStripSeparator(),
                cut, copy, paste, delete,
                new ToolStripSeparator(),
                selectAll,
                new ToolStripSeparator(),
                toggleComment, duplicateLine,
                new ToolStripSeparator(),
                fixIndentation, trimWhitespace
            ]);

            menu.Opening += (_, _) =>
            {
                bool hasSel = Editor.SelectionLength > 0;
                cut.Enabled    = hasSel;
                copy.Enabled   = hasSel;
                delete.Enabled = hasSel;
                paste.Enabled  = Clipboard.ContainsText();
                undo.Enabled   = Editor.CanUndo;
                redo.Enabled   = Editor.CanRedo;
            };

            return menu;
        }

        private static bool IsCSharpFile(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext == ".cs" || ext == ".csx";
        }


        public void LoadContent()
        {
            if (!File.Exists(FilePath)) return;
            Editor.TextChanged -= Editor_TextChanged;
            _originalContent = File.ReadAllText(FilePath);
            Editor.Text = _originalContent;
            Editor.TextChanged += Editor_TextChanged;
            IsModified = false;
            UpdateTabTitle();
            if (_roslyn != null)
            {
                _roslyn.UpdateText(_originalContent);
                _ = ApplySyntaxHighlightingAsync();
            }
        }

        public void LoadContent(string content)
        {
            Editor.TextChanged -= Editor_TextChanged;
            _originalContent = content;
            Editor.Text = content;
            Editor.TextChanged += Editor_TextChanged;
            IsModified = false;
            UpdateTabTitle();
            if (_roslyn != null)
            {
                _roslyn.UpdateText(content);
                _ = ApplySyntaxHighlightingAsync();
            }
        }

        public bool SaveContent()
        {
            try
            {
                var dir = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllText(FilePath, Editor.Text);
                _originalContent = Editor.Text;
                IsModified = false;
                UpdateTabTitle();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void Editor_TextChanged(object? sender, EventArgs e)
        {
            IsModified = Editor.Text != _originalContent;
            UpdateTabTitle();

            if (_roslyn != null)
            {
                _roslyn.UpdateText(Editor.Text);
                _highlightTimer.Stop();
                _highlightTimer.Start();

                // Trigger completion when caret is inside/after an identifier OR just after '.'
                var caret = Editor.SelectionStart;
                var text  = Editor.Text;
                bool inIdentifier = caret > 0 && (char.IsLetterOrDigit(text[caret - 1]) || text[caret - 1] == '_');
                bool afterDot     = caret > 0 && text[caret - 1] == '.';

                // Detect "Console.W" — inside a word that sits right after a dot
                bool inMemberAccessWord = false;
                if (inIdentifier)
                {
                    int ws = caret;
                    while (ws > 0 && (char.IsLetterOrDigit(text[ws - 1]) || text[ws - 1] == '_'))
                        ws--;
                    inMemberAccessWord = ws > 0 && text[ws - 1] == '.';
                }

                if (afterDot || inIdentifier)
                {
                    if (_completionPopup.Visible && inIdentifier && !inMemberAccessWord)
                    {
                        // Narrowing a plain identifier — filter in-place, no re-fetch needed
                        _ = ShowCompletionsAsync(forceRefetch: false);
                    }
                    else
                    {
                        // After a dot, inside a member-access word, or popup not open — debounce then fetch
                        _completionTimer.Stop();
                        _completionTimer.Start();
                    }
                }
                else
                {
                    _completionTimer.Stop();
                    _completionPopup.Hide();
                }
            }
        }

        private void Editor_KeyDown(object? sender, KeyEventArgs e)
        {
            // Route navigation keys into the popup when it is visible
            if (_completionPopup.Visible)
            {
                if (e.KeyCode == Keys.Down)  { e.SuppressKeyPress = true; _completionPopup.SelectNext(); return; }
                if (e.KeyCode == Keys.Up)    { e.SuppressKeyPress = true; _completionPopup.SelectPrev(); return; }
                if (e.KeyCode == Keys.Escape){ e.SuppressKeyPress = true; _completionPopup.Hide();       return; }
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    e.SuppressKeyPress = true;
                    _completionPopup.CommitSelected();
                    return;
                }
            }

            if (e.KeyCode == Keys.Tab && !e.Shift)
            {
                e.SuppressKeyPress = true;
                Editor.SelectedText = "    ";
                return;
            }

            if (e.KeyCode == Keys.Tab && e.Shift)
            {
                e.SuppressKeyPress = true;
                RemoveIndentFromSelection();
                return;
            }

            if (e.KeyCode == Keys.Enter && !e.Modifiers.HasFlag(Keys.Control))
            {
                e.SuppressKeyPress = true;
                _completionPopup.Hide();
                InsertNewlineWithIndent();
                return;
            }

            // Ctrl+Space: force completion
            if (e.KeyCode == Keys.Space && e.Control)
            {
                e.SuppressKeyPress = true;
                _ = ShowCompletionsAsync(forceRefetch: true);
                return;
            }

            // Ctrl+/ : toggle line comment
            if (e.KeyCode == Keys.Oem2 && e.Control && !e.Shift && !e.Alt)
            {
                e.SuppressKeyPress = true;
                ToggleLineComment();
                return;
            }

            // Ctrl+D : duplicate line
            if (e.KeyCode == Keys.D && e.Control && !e.Shift && !e.Alt)
            {
                e.SuppressKeyPress = true;
                DuplicateCurrentLine();
            }
        }

        private void Editor_KeyPress(object? sender, KeyPressEventArgs e)
        {
            _lastTypedChar = e.KeyChar;

            if (_completionPopup.Visible)
            {
                // Hide on characters that unambiguously end an expression
                if (char.IsWhiteSpace(e.KeyChar) || e.KeyChar == ';')
                    _completionPopup.Hide();
            }
        }

        private void Editor_MouseDown(object? sender, MouseEventArgs e)
        {
            // Clicking anywhere in the editor dismisses the completion popup
            // because the caret is about to move to a new position.
            if (_completionPopup.Visible)
            {
                _completionTimer.Stop();
                _completionPopup.Hide();
            }
        }


        private void InsertNewlineWithIndent()
        {
            var caretPos = Editor.SelectionStart;
            var text = Editor.Text;

            // Find the start of the current line
            var lineStart = caretPos == 0 ? 0 : text.LastIndexOf('\n', caretPos - 1) + 1;

            // Collect leading whitespace from the current line
            var indent = new System.Text.StringBuilder();
            for (int i = lineStart; i < text.Length && i < caretPos; i++)
            {
                if (text[i] == ' ' || text[i] == '\t')
                    indent.Append(text[i]);
                else
                    break;
            }

            // Check if the character immediately before the caret (ignoring trailing spaces) is '{'
            var trimmedBefore = text[lineStart..caretPos].TrimEnd();
            bool openBrace = trimmedBefore.EndsWith('{');

            // Check if the character at (or after) the caret is '}'
            var nextNonWhitespace = caretPos;
            while (nextNonWhitespace < text.Length && text[nextNonWhitespace] == ' ')
                nextNonWhitespace++;
            bool closeBrace = nextNonWhitespace < text.Length && text[nextNonWhitespace] == '}';

            if (openBrace && closeBrace)
            {
                // Place cursor on its own indented line between braces, and push the closing brace down
                var innerIndent = indent + "    ";
                Editor.SelectedText = "\n" + innerIndent + "\n" + indent;
                // Move caret to end of the inner indent line
                Editor.SelectionStart = caretPos + 1 + innerIndent.Length;
                Editor.SelectionLength = 0;
            }
            else if (openBrace)
            {
                Editor.SelectedText = "\n" + indent + "    ";
            }
            else
            {
                Editor.SelectedText = "\n" + indent;
            }
        }

        private void RemoveIndentFromSelection()
        {
            var text = Editor.Text;
            var selStart = Editor.SelectionStart;
            var lineStart = selStart == 0 ? 0 : text.LastIndexOf('\n', selStart - 1) + 1;

            if (lineStart >= text.Length) return;

            // Count leading spaces (up to 4) to remove
            int removed = 0;
            for (int i = lineStart; i < text.Length && removed < 4; i++)
            {
                if (text[i] == ' ') removed++;
                else if (text[i] == '\t') { removed = 4; break; }
                else break;
            }

            if (removed == 0) return;

            Editor.Select(lineStart, removed);
            Editor.SelectedText = string.Empty;
        }

        private void UpdateTabTitle()
        {
            var title = Path.GetFileName(FilePath);
            if (IsModified)
                title += " *";
            if (Text != title)
                Text = title;
        }

        private void ToggleLineComment()
        {
            var text = Editor.Text;
            var selStart = Editor.SelectionStart;
            var selEnd   = selStart + Editor.SelectionLength;

            int firstLine = Editor.GetLineFromCharIndex(selStart);
            int lastLine  = Editor.GetLineFromCharIndex(Math.Max(selEnd - 1, selStart));

            // Determine whether to add or remove: if ALL selected lines start with
            // "//" (ignoring leading whitespace), remove; otherwise add.
            bool allCommented = true;
            for (int i = firstLine; i <= lastLine && i < Editor.Lines.Length; i++)
            {
                if (!Editor.Lines[i].TrimStart().StartsWith("//"))
                { allCommented = false; break; }
            }

            Editor.TextChanged -= Editor_TextChanged;
            var scrollPos = Point.Empty;
            NativeMethods.SendMessage(Editor.Handle, NativeMethods.EM_GETSCROLLPOS, 0, ref scrollPos);
            NativeMethods.SendMessage(Editor.Handle, NativeMethods.WM_SETREDRAW, 0, 0);
            try
            {
                var sb = new StringBuilder();
                for (int i = firstLine; i <= lastLine && i < Editor.Lines.Length; i++)
                {
                    var line = Editor.Lines[i];
                    if (allCommented)
                    {
                        int idx = line.IndexOf("//", StringComparison.Ordinal);
                        if (idx >= 0)
                            line = line.Remove(idx, line.Length > idx + 2 && line[idx + 2] == ' ' ? 3 : 2);
                    }
                    else
                    {
                        int ws = 0;
                        while (ws < line.Length && line[ws] == ' ') ws++;
                        line = line.Insert(ws, "// ");
                    }
                    if (i > firstLine) sb.Append('\n');
                    sb.Append(line);
                }

                int blockStart = Editor.GetFirstCharIndexFromLine(firstLine);
                int blockEnd   = lastLine < Editor.Lines.Length - 1
                    ? Editor.GetFirstCharIndexFromLine(lastLine + 1) - 1
                    : text.Length;

                Editor.Select(blockStart, blockEnd - blockStart);
                Editor.SelectedText = sb.ToString();
                Editor.Select(blockStart, sb.Length);
            }
            finally
            {
                NativeMethods.SendMessage(Editor.Handle, NativeMethods.WM_SETREDRAW, 1, 0);
                NativeMethods.SendMessage(Editor.Handle, NativeMethods.EM_SETSCROLLPOS, 0, ref scrollPos);
                Editor.TextChanged += Editor_TextChanged;
                Editor.Refresh();
            }

            // Trigger modified state + re-highlight
            Editor_TextChanged(this, EventArgs.Empty);
        }

        private void DuplicateCurrentLine()
        {
            int line = Editor.GetLineFromCharIndex(Editor.SelectionStart);
            if (line >= Editor.Lines.Length) return;

            int lineStart = Editor.GetFirstCharIndexFromLine(line);
            var lineText  = Editor.Lines[line];

            int insertAt = line < Editor.Lines.Length - 1
                ? Editor.GetFirstCharIndexFromLine(line + 1)
                : Editor.Text.Length;

            bool needsNewline = insertAt == Editor.Text.Length;

            Editor.TextChanged -= Editor_TextChanged;
            var scrollPos = Point.Empty;
            NativeMethods.SendMessage(Editor.Handle, NativeMethods.EM_GETSCROLLPOS, 0, ref scrollPos);
            NativeMethods.SendMessage(Editor.Handle, NativeMethods.WM_SETREDRAW, 0, 0);
            try
            {
                Editor.Select(insertAt, 0);
                Editor.SelectedText = (needsNewline ? "\n" : "") + lineText + (!needsNewline ? "\n" : "");
            }
            finally
            {
                NativeMethods.SendMessage(Editor.Handle, NativeMethods.WM_SETREDRAW, 1, 0);
                NativeMethods.SendMessage(Editor.Handle, NativeMethods.EM_SETSCROLLPOS, 0, ref scrollPos);
                Editor.TextChanged += Editor_TextChanged;
                Editor.Refresh();
            }

            Editor_TextChanged(this, EventArgs.Empty);
        }

        private void FixIndentation()
        {
            var lines = Editor.Lines;
            var sb    = new StringBuilder();
            int depth = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                var trimmed = lines[i].Trim();
                if (trimmed.Length == 0)
                {
                    if (i > 0) sb.Append('\n');
                    continue;
                }

                // Decrease indent before lines starting with '}'
                if (trimmed.StartsWith('}'))
                    depth = Math.Max(depth - 1, 0);

                if (i > 0) sb.Append('\n');
                sb.Append(new string(' ', depth * 4));
                sb.Append(trimmed);

                // Increase indent after lines ending with '{'
                if (trimmed.EndsWith('{'))
                    depth++;
            }

            var selStart = Editor.SelectionStart;
            Editor.TextChanged -= Editor_TextChanged;
            var scrollPos = Point.Empty;
            NativeMethods.SendMessage(Editor.Handle, NativeMethods.EM_GETSCROLLPOS, 0, ref scrollPos);
            NativeMethods.SendMessage(Editor.Handle, NativeMethods.WM_SETREDRAW, 0, 0);
            try
            {
                Editor.Text = sb.ToString();
                Editor.SelectionStart = Math.Min(selStart, Editor.Text.Length);
            }
            finally
            {
                NativeMethods.SendMessage(Editor.Handle, NativeMethods.WM_SETREDRAW, 1, 0);
                NativeMethods.SendMessage(Editor.Handle, NativeMethods.EM_SETSCROLLPOS, 0, ref scrollPos);
                Editor.TextChanged += Editor_TextChanged;
                Editor.Refresh();
            }

            Editor_TextChanged(this, EventArgs.Empty);
        }

        private void TrimTrailingWhitespace()
        {
            var lines = Editor.Lines;
            var sb    = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0) sb.Append('\n');
                sb.Append(lines[i].TrimEnd());
            }

            var selStart2 = Editor.SelectionStart;
            Editor.TextChanged -= Editor_TextChanged;
            var scrollPos2 = Point.Empty;
            NativeMethods.SendMessage(Editor.Handle, NativeMethods.EM_GETSCROLLPOS, 0, ref scrollPos2);
            NativeMethods.SendMessage(Editor.Handle, NativeMethods.WM_SETREDRAW, 0, 0);
            try
            {
                Editor.Text = sb.ToString();
                Editor.SelectionStart = Math.Min(selStart2, Editor.Text.Length);
            }
            finally
            {
                NativeMethods.SendMessage(Editor.Handle, NativeMethods.WM_SETREDRAW, 1, 0);
                NativeMethods.SendMessage(Editor.Handle, NativeMethods.EM_SETSCROLLPOS, 0, ref scrollPos2);
                Editor.TextChanged += Editor_TextChanged;
                Editor.Refresh();
            }

            Editor_TextChanged(this, EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _highlightTimer.Dispose();
                _completionTimer.Dispose();
                _completionPopup.Dispose();
                _editorContextMenu.Dispose();
                _roslyn?.Dispose();
                _highlightCts.Dispose();
                _completionCts.Dispose();
            }
            base.Dispose(disposing);
        }

        private void CompletionPopup_ItemCommitted(object? sender, CompletionCandidate item)
        {
            var text = Editor.Text;
            var caret = Editor.SelectionStart;
            int wordStart = caret;
            while (wordStart > 0 && (char.IsLetterOrDigit(text[wordStart - 1]) || text[wordStart - 1] == '_'))
                wordStart--;

            Editor.Select(wordStart, caret - wordStart);
            Editor.SelectedText = item.DisplayText;
        }

        private async Task ShowCompletionsAsync(bool forceRefetch = false)
        {
            if (_roslyn == null) return;

            var text  = Editor.Text;
            var caret = Editor.SelectionStart;

            // Compute the partial word before the caret
            int wordStart = caret;
            while (wordStart > 0 && (char.IsLetterOrDigit(text[wordStart - 1]) || text[wordStart - 1] == '_'))
                wordStart--;
            var prefix = caret > wordStart ? text[wordStart..caret] : string.Empty;

            // Dot-trigger only when caret is IMMEDIATELY after the dot (no word between).
            // When the caret is inside "Console.W", wordStart points at 'W' and
            // text[wordStart-1]=='.', but we must use Invoke — not dot-trigger — so
            // Roslyn knows the dot was already committed and we're filtering members.
            bool caretRightAfterDot = caret > 0 && text[caret - 1] == '.';
            bool inMemberAccessWord = !caretRightAfterDot && wordStart > 0 && text[wordStart - 1] == '.';
            char triggerChar = caretRightAfterDot ? '.' : '\0';

            // Any member-access context always needs a fresh Roslyn fetch
            if (caretRightAfterDot || inMemberAccessWord) forceRefetch = true;

            // If the popup is already showing and we're just narrowing the same word,
            // filter the existing list in-place — no Roslyn round-trip needed.
            if (_completionPopup.Visible && !forceRefetch)
            {
                _completionPopup.FilterBy(prefix);
                if (!_completionPopup.HasItems)
                    _completionPopup.Hide();
                return;
            }

            // Full fetch from Roslyn
            _completionCts.Cancel();
            _completionCts = new CancellationTokenSource();
            var ct = _completionCts.Token;

            try
            {
                var items = await _roslyn.GetCompletionsAsync(caret, triggerChar, ct);
                if (ct.IsCancellationRequested) return;

                var filtered = string.IsNullOrEmpty(prefix)
                    ? items
                    : items.Where(i => i.DisplayText.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();

                if (filtered.Count == 0)
                {
                    _completionPopup.Hide();
                    return;
                }

                var caretPos  = Editor.GetPositionFromCharIndex(Editor.SelectionStart);
                var screenPos = Editor.PointToScreen(caretPos);
                screenPos.Y  += Editor.Font.Height + 2;

                _completionPopup.SetItems(filtered);
                _completionPopup.Location = screenPos;
                if (!_completionPopup.Visible)
                    _completionPopup.Show(Editor.FindForm()!);
            }
            catch (OperationCanceledException) { }
            catch { }
        }

        // Priority order for classification: higher-priority types come first.
        // When Roslyn returns overlapping spans (e.g. StaticSymbol over MethodName),
        // the first match wins per character, preventing strings/comments from being
        // overwritten by secondary classifications like StaticSymbol or Punctuation.
        private static readonly string[] _classificationPriority =
        [
            ClassificationTypeNames.Comment,
            ClassificationTypeNames.XmlDocCommentText,
            ClassificationTypeNames.XmlDocCommentDelimiter,
            ClassificationTypeNames.XmlDocCommentName,
            ClassificationTypeNames.XmlDocCommentAttributeName,
            ClassificationTypeNames.ExcludedCode,
            ClassificationTypeNames.StringLiteral,
            ClassificationTypeNames.VerbatimStringLiteral,
            ClassificationTypeNames.StringEscapeCharacter,
            ClassificationTypeNames.NumericLiteral,
            ClassificationTypeNames.PreprocessorKeyword,
            ClassificationTypeNames.PreprocessorText,
            ClassificationTypeNames.ControlKeyword,
            ClassificationTypeNames.Keyword,
            ClassificationTypeNames.ClassName,
            ClassificationTypeNames.RecordClassName,
            ClassificationTypeNames.StructName,
            ClassificationTypeNames.RecordStructName,
            ClassificationTypeNames.InterfaceName,
            ClassificationTypeNames.EnumName,
            ClassificationTypeNames.DelegateName,
            ClassificationTypeNames.TypeParameterName,
            ClassificationTypeNames.EnumMemberName,
            ClassificationTypeNames.ConstantName,
            ClassificationTypeNames.MethodName,
            ClassificationTypeNames.ExtensionMethodName,
            ClassificationTypeNames.OperatorOverloaded,
            ClassificationTypeNames.PropertyName,
            ClassificationTypeNames.FieldName,
            ClassificationTypeNames.EventName,
            ClassificationTypeNames.LocalName,
            ClassificationTypeNames.ParameterName,
            ClassificationTypeNames.NamespaceName,
            ClassificationTypeNames.LabelName,
            ClassificationTypeNames.Operator,
            ClassificationTypeNames.StaticSymbol,
        ];

        public async Task ApplySyntaxHighlightingAsync()
        {
            if (_roslyn == null) return;

            // Cancel any in-flight highlight pass and get a fresh token
            var cts = new CancellationTokenSource();
            var old = Interlocked.Exchange(ref _highlightCts, cts);
            old.Cancel();
            old.Dispose();
            var ct = cts.Token;

            var snapshot = Editor.Text;
            _roslyn.UpdateText(snapshot);

            IReadOnlyList<Microsoft.CodeAnalysis.Classification.ClassifiedSpan> spans;
            try
            {
                spans = await _roslyn.GetClassifiedSpansAsync(ct).ConfigureAwait(true);
            }
            catch (OperationCanceledException) { return; }
            catch { return; }

            if (ct.IsCancellationRequested) return;
            if (Editor.Text != snapshot) return;

            // Build a per-character color index.
            // Priority ordering ensures strings/comments are never overwritten.
            var textLen = snapshot.Length;
            var colorIndex = new int[textLen]; // 0 = default

            // Build priority lookup: classification name -> priority (lower = higher priority)
            var priorityLookup = new Dictionary<string, int>(_classificationPriority.Length);
            for (int i = 0; i < _classificationPriority.Length; i++)
                priorityLookup[_classificationPriority[i]] = i + 1;

            // Collect unique colors used and assign indices
            // Index 0 = _defaultColor
            var colorList = new List<Color> { _defaultColor };
            var colorMap = new Dictionary<Color, int> { [_defaultColor] = 0 };
            var charPriority = new int[textLen]; // track priority per char (0 = unset)

            foreach (var span in spans)
            {
                if (span.TextSpan.End > textLen) continue;
                if (!_classificationColors.TryGetValue(span.ClassificationType, out var color))
                    continue;
                if (!priorityLookup.TryGetValue(span.ClassificationType, out var pri))
                    pri = 999;

                if (!colorMap.TryGetValue(color, out int ci))
                {
                    ci = colorList.Count;
                    colorMap[color] = ci;
                    colorList.Add(color);
                }

                for (int i = span.TextSpan.Start; i < span.TextSpan.End; i++)
                {
                    if (charPriority[i] == 0 || pri < charPriority[i])
                    {
                        charPriority[i] = pri;
                        colorIndex[i] = ci;
                    }
                }
            }

            // Build RTF color table
            var rtf = new StringBuilder(textLen * 2 + 512);
            rtf.Append(@"{\rtf1\ansi\deff0");

            // Font table - single font
            var fontName = Editor.Font.FontFamily.Name;
            rtf.Append(@"{\fonttbl{\f0 ").Append(fontName).Append(";}}\n");

            // Color table
            rtf.Append(@"{\colortbl ;");
            foreach (var c in colorList)
                rtf.Append(@"\red").Append(c.R).Append(@"\green").Append(c.G).Append(@"\blue").Append(c.B).Append(';');
            rtf.Append("}\n");

            // Background color
            rtf.Append(@"\viewbksp1");

            // Font size in half-points
            int halfPts = (int)(Editor.Font.SizeInPoints * 2);
            rtf.Append(@"\f0\fs").Append(halfPts).Append(' ');

            // Default foreground is color index 1 (0-based in \cf is 1-based: \cf1 = colorList[0])
            rtf.Append(@"\cf1 ");

            // Emit text with color changes
            int currentCI = 0; // default
            for (int i = 0; i < textLen; i++)
            {
                int ci = colorIndex[i];
                if (ci != currentCI)
                {
                    // RTF \cf is 1-based (cf1 = first color in table)
                    rtf.Append(@"\cf").Append(ci + 1).Append(' ');
                    currentCI = ci;
                }

                char ch = snapshot[i];
                switch (ch)
                {
                    case '\\': rtf.Append(@"\\"); break;
                    case '{':  rtf.Append(@"\{"); break;
                    case '}':  rtf.Append(@"\}"); break;
                    case '\r': break; // skip \r, handle \n
                    case '\n': rtf.Append(@"\par "); break;
                    case '\t': rtf.Append(@"\tab "); break;
                    default:
                        if (ch > 127)
                            rtf.Append(@"\u").Append((int)ch).Append('?');
                        else
                            rtf.Append(ch);
                        break;
                }
            }

            rtf.Append('}');

            // Apply to editor
            var selStart = Editor.SelectionStart;
            var selLen = Editor.SelectionLength;

            Editor.TextChanged -= Editor_TextChanged;

            var scrollPos = Point.Empty;
            NativeMethods.SendMessage(Editor.Handle, NativeMethods.EM_GETSCROLLPOS, 0, ref scrollPos);

            NativeMethods.SendMessage(Editor.Handle, NativeMethods.WM_SETREDRAW, 0, 0);
            try
            {
                Editor.Rtf = rtf.ToString();
                Editor.Select(selStart, selLen);
                Editor.SelectionColor = _defaultColor;
            }
            finally
            {
                NativeMethods.SendMessage(Editor.Handle, NativeMethods.WM_SETREDRAW, 1, 0);
                NativeMethods.SendMessage(Editor.Handle, NativeMethods.EM_SETSCROLLPOS, 0, ref scrollPos);
                Editor.TextChanged += Editor_TextChanged;
                Editor.Invalidate();
            }
        }

        // Synchronous shim kept for any existing callers
        public void ApplySyntaxHighlighting() => _ = ApplySyntaxHighlightingAsync();
    }


    internal sealed class LineNumberGutter : Control
    {
        private readonly RichTextBox _editor;
        private const int Padding = 4;

        public LineNumberGutter(RichTextBox editor)
        {
            _editor = editor;
            BackColor = Color.FromArgb(30, 30, 30);
            ForeColor = Color.FromArgb(100, 100, 100);
            Font = editor.Font;
            DoubleBuffered = true;
            Cursor = Cursors.Default;

            _editor.TextChanged += (_, _) => RefreshGutter();
            _editor.VScroll += (_, _) => Invalidate();
            _editor.FontChanged += (_, _) =>
            {
                Font = _editor.Font;
                RefreshGutter();
            };
        }

        public void RefreshGutter()
        {
            Width = MeasureWidth();
            Invalidate();
        }

        private int MeasureWidth()
        {
            int lineCount = Math.Max(_editor.Lines.Length, 1);
            int digits = lineCount.ToString().Length;
            using var g = CreateGraphics();
            int charWidth = (int)g.MeasureString(new string('9', Math.Max(digits, 2)), Font).Width;
            return charWidth + Padding * 3;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.Clear(BackColor);

            // Right-side separator line
            using var sepPen = new Pen(Color.FromArgb(50, 50, 50));
            g.DrawLine(sepPen, Width - 1, 0, Width - 1, Height);

            if (_editor.Lines.Length == 0) return;

            int lineHeight = _editor.Font.Height;
            if (lineHeight <= 0) return;

            // Determine first and last visible lines from the editor viewport
            int firstChar = _editor.GetCharIndexFromPosition(new Point(0, 0));
            int firstLine = _editor.GetLineFromCharIndex(firstChar);

            int lastChar = _editor.GetCharIndexFromPosition(new Point(0, _editor.ClientSize.Height - 1));
            int lastLine = _editor.GetLineFromCharIndex(lastChar);

            // Active line for highlight
            int currentLine = _editor.GetLineFromCharIndex(_editor.SelectionStart);

            using var normalBrush = new SolidBrush(ForeColor);
            using var activeBrush = new SolidBrush(Color.FromArgb(200, 200, 200));
            using var activeBackBrush = new SolidBrush(Color.FromArgb(42, 42, 42));
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            };

            for (int i = firstLine; i <= lastLine && i < _editor.Lines.Length; i++)
            {
                int charIndex = _editor.GetFirstCharIndexFromLine(i);
                if (charIndex < 0) break;

                var pos = _editor.GetPositionFromCharIndex(charIndex);
                var rect = new Rectangle(0, pos.Y, Width - 1, lineHeight);

                if (i == currentLine)
                {
                    g.FillRectangle(activeBackBrush, rect);
                    g.DrawString((i + 1).ToString(), Font, activeBrush,
                        new RectangleF(Padding, pos.Y, Width - Padding * 2, lineHeight), sf);
                }
                else
                {
                    g.DrawString((i + 1).ToString(), Font, normalBrush,
                        new RectangleF(Padding, pos.Y, Width - Padding * 2, lineHeight), sf);
                }
            }

            sf.Dispose();
        }
    }


    internal sealed class EditorContextMenuRenderer : ToolStripProfessionalRenderer
    {
        public EditorContextMenuRenderer()
            : base(new EditorContextMenuColorTable()) { }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Enabled
                ? Color.FromArgb(220, 220, 220)
                : Color.FromArgb(100, 100, 100);
            base.OnRenderItemText(e);
        }

        private sealed class EditorContextMenuColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected              => Color.FromArgb(62, 62, 64);
            public override Color MenuItemSelectedGradientBegin => Color.FromArgb(62, 62, 64);
            public override Color MenuItemSelectedGradientEnd   => Color.FromArgb(62, 62, 64);
            public override Color MenuItemBorder                => Color.FromArgb(62, 62, 64);
            public override Color MenuBorder                    => Color.FromArgb(50, 50, 50);
            public override Color ToolStripDropDownBackground   => Color.FromArgb(37, 37, 38);
            public override Color ImageMarginGradientBegin      => Color.FromArgb(37, 37, 38);
            public override Color ImageMarginGradientMiddle     => Color.FromArgb(37, 37, 38);
            public override Color ImageMarginGradientEnd        => Color.FromArgb(37, 37, 38);
            public override Color SeparatorDark                 => Color.FromArgb(55, 55, 55);
            public override Color SeparatorLight                => Color.FromArgb(55, 55, 55);
        }
    }
}
