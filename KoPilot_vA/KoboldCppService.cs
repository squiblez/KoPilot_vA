using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace KoPilot_vA
{
    public class KoboldCppService
    {
        public string EndpointUrl { get; set; } = "http://192.168.20.227:5001";
        public int MaxTokens { get; set; } = 4096;
        public int ContextLength { get; set; } = 250000;
        public double Temperature { get; set; } = 0.7;
        public string SystemPrompt { get; set; } = DefaultSystemPrompt;
        public bool IsEnabled { get; set; } = true;

        private readonly HttpClient _client;

        private const string DefaultSystemPrompt =
@"You are an expert C# coding assistant embedded in the KoPilot IDE.
You have full access to the user's project source files which are provided below each message.

When the user asks you to modify code, you MUST reply with the complete updated file contents wrapped in special markers so the IDE can save them automatically.

Use this exact format for EACH file you want to create or modify:

<<<FILE:relative/path/to/File.cs>>>
(entire file content here)
<<<END_FILE>>>

Rules:
- Always output the COMPLETE file content, never partial snippets or diffs.
- Use the relative path from the project root (forward slashes).
- You may output multiple <<<FILE>>>...<<<END_FILE>>> blocks in one response.
- If the user only asks a question (no code changes needed), just answer normally without any FILE blocks.
- Keep explanations brief and outside the FILE blocks.";

        public KoboldCppService()
        {
            _client = new HttpClient { Timeout = TimeSpan.FromMinutes(30) };
        }

        /// <summary>
        /// Builds a context block containing every project file's content,
        /// formatted so the AI model can see the full codebase.
        /// </summary>
        public static string BuildProjectContext(KoPilotProject project)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== PROJECT CONTEXT ===");
            sb.AppendLine($"Project: {project.Name}");
            sb.AppendLine($"Language: {project.Language}");
            sb.AppendLine($"Framework: {project.TargetFramework}");
            sb.AppendLine($"Files: {project.Files.Count}");
            sb.AppendLine();

            foreach (var relPath in project.Files.OrderBy(f => f))
            {
                var absPath = project.GetAbsolutePath(relPath);
                if (!File.Exists(absPath)) continue;

                var ext = Path.GetExtension(absPath).ToLowerInvariant();
                // Include source and config files, skip binaries
                if (ext is ".exe" or ".dll" or ".pdb" or ".obj" or ".cache" or ".nupkg") continue;

                try
                {
                    var content = File.ReadAllText(absPath);
                    sb.AppendLine($"--- FILE: {relPath} ---");
                    sb.AppendLine(content);
                    sb.AppendLine($"--- END: {relPath} ---");
                    sb.AppendLine();
                }
                catch
                {
                    sb.AppendLine($"--- FILE: {relPath} (unreadable) ---");
                    sb.AppendLine();
                }
            }

            sb.AppendLine("=== END PROJECT CONTEXT ===");
            return sb.ToString();
        }

        /// <summary>
        /// Sends a message to the KoboldCpp endpoint with full project context.
        /// </summary>
        public async Task<string> GenerateAsync(string userMessage, string? projectContext = null,
            string? activeFileName = null, CancellationToken cancellationToken = default)
        {
            if (!IsEnabled)
                return "[AI endpoint is disabled]";

            try
            {
                var generateUrl = EndpointUrl.TrimEnd('/') + "/api/v1/generate";

                var promptBuilder = new StringBuilder();

                // System prompt
                if (!string.IsNullOrWhiteSpace(SystemPrompt))
                {
                    promptBuilder.AppendLine(SystemPrompt);
                    promptBuilder.AppendLine();
                }

                // Project context
                if (!string.IsNullOrEmpty(projectContext))
                {
                    promptBuilder.AppendLine(projectContext);
                    promptBuilder.AppendLine();
                }

                // Active file hint
                if (!string.IsNullOrEmpty(activeFileName))
                {
                    promptBuilder.AppendLine($"[Currently open file: {activeFileName}]");
                    promptBuilder.AppendLine();
                }

                promptBuilder.AppendLine($"User: {userMessage}");
                promptBuilder.AppendLine();
                promptBuilder.AppendLine("Assistant:");

                var fullPrompt = promptBuilder.ToString();

                var requestBody = new
                {
                    prompt = fullPrompt,
                    max_length = MaxTokens,
                    max_context_length = ContextLength,
                    temperature = Temperature,
                    top_p = 0.9,
                    rep_pen = 1.1
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(generateUrl, content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(responseJson);

                if (doc.RootElement.TryGetProperty("results", out var results) &&
                    results.GetArrayLength() > 0)
                {
                    return results[0].GetProperty("text").GetString()?.Trim() ?? string.Empty;
                }

                return "[No response from AI]";
            }
            catch (OperationCanceledException)
            {
                return "[Request cancelled]";
            }
            catch (Exception ex)
            {
                return $"[AI Error: {ex.Message}]";
            }
        }

        /// <summary>
        /// Parses <<<FILE:path>>>...<<<END_FILE>>> blocks from an AI response
        /// and writes each one to disk under the project root.
        /// Returns a list of relative paths that were written.
        /// </summary>
        public static List<string> ApplyFileEdits(string aiResponse, KoPilotProject project)
        {
            var written = new List<string>();
            if (string.IsNullOrEmpty(aiResponse)) return written;

            // Match <<<FILE:some/path.cs>>>\n...content...\n<<<END_FILE>>>
            var pattern = @"<<<FILE:(.+?)>>>\s*\n(.*?)<<<END_FILE>>>";
            var matches = Regex.Matches(aiResponse, pattern, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                var relPath = match.Groups[1].Value.Trim();
                var fileContent = match.Groups[2].Value;

                // Remove a single trailing newline before the marker if present
                if (fileContent.EndsWith("\n"))
                    fileContent = fileContent[..^1];

                try
                {
                    var absPath = project.GetAbsolutePath(relPath);
                    var dir = Path.GetDirectoryName(absPath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    File.WriteAllText(absPath, fileContent);

                    // Ensure the file is tracked in the project
                    project.AddFile(relPath);

                    written.Add(relPath);
                }
                catch
                {
                    // Skip files that can't be written
                }
            }

            if (written.Count > 0)
                project.Save();

            return written;
        }

        /// <summary>
        /// Returns just the "plain text" portion of an AI response with
        /// FILE blocks stripped out, for display in the chat pane.
        /// </summary>
        public static string StripFileBlocks(string aiResponse)
        {
            if (string.IsNullOrEmpty(aiResponse)) return aiResponse;
            var stripped = Regex.Replace(aiResponse,
                @"<<<FILE:.+?>>>.*?<<<END_FILE>>>",
                "[file saved]",
                RegexOptions.Singleline);
            return stripped.Trim();
        }
    }
}
