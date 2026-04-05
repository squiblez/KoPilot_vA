using System.Text;
using System.Text.Json;

namespace KoPilot_vA
{
    public sealed class ChatMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public sealed class ChatHistory
    {
        public List<ChatMessage> Messages { get; set; } = new();

        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public void Add(string role, string text)
        {
            Messages.Add(new ChatMessage { Role = role, Text = text, Timestamp = DateTime.UtcNow });
        }

        public void Clear() => Messages.Clear();

        public void Save(string filePath)
        {
            var json = JsonSerializer.Serialize(this, _jsonOptions);
            File.WriteAllText(filePath, json);
        }

        public static ChatHistory Load(string filePath)
        {
            if (!File.Exists(filePath))
                return new ChatHistory();

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<ChatHistory>(json) ?? new ChatHistory();
            }
            catch
            {
                return new ChatHistory();
            }
        }

        public static string GetHistoryPath(KoPilotProject project)
        {
            return Path.Combine(project.RootPath, project.Name + ".chat.json");
        }

        public string ExportToText()
        {
            var sb = new StringBuilder();
            foreach (var msg in Messages)
            {
                sb.AppendLine($"[{msg.Timestamp:yyyy-MM-dd HH:mm:ss}] {msg.Role}:");
                sb.AppendLine(msg.Text);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Imports a chat history from a file. Supports both JSON (.json / .chat.json)
        /// and the plain-text format produced by <see cref="ExportToText"/>.
        /// </summary>
        public static ChatHistory ImportFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return new ChatHistory();

            var content = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(content))
                return new ChatHistory();

            // Try JSON first (matches the native .chat.json format)
            var trimmed = content.TrimStart();
            if (trimmed.StartsWith('{'))
            {
                try
                {
                    var history = JsonSerializer.Deserialize<ChatHistory>(content);
                    if (history != null && history.Messages.Count > 0)
                        return history;
                }
                catch { }
            }

            // Fall back to plain-text format:
            // [yyyy-MM-dd HH:mm:ss] Role:
            // message text (may span multiple lines)
            //
            // (blank line separates entries)
            return ParseTextFormat(content);
        }

        private static ChatHistory ParseTextFormat(string text)
        {
            var history = new ChatHistory();
            var lines = text.Split('\n');

            string? currentRole = null;
            DateTime currentTimestamp = DateTime.UtcNow;
            var bodyBuilder = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].TrimEnd('\r');

                // Try to match a header line: [2024-01-15 12:30:45] Role:
                var match = System.Text.RegularExpressions.Regex.Match(
                    line, @"^\[(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})\]\s+(.+):$");

                if (match.Success)
                {
                    // Flush the previous message if we had one
                    if (currentRole != null)
                    {
                        history.Messages.Add(new ChatMessage
                        {
                            Role = currentRole,
                            Text = bodyBuilder.ToString().TrimEnd(),
                            Timestamp = currentTimestamp
                        });
                    }

                    // Start a new message
                    if (DateTime.TryParseExact(match.Groups[1].Value,
                            "yyyy-MM-dd HH:mm:ss",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.AssumeUniversal,
                            out var ts))
                        currentTimestamp = ts;
                    else
                        currentTimestamp = DateTime.UtcNow;

                    currentRole = match.Groups[2].Value.Trim();
                    bodyBuilder.Clear();
                }
                else if (currentRole != null)
                {
                    // Accumulate body lines
                    if (bodyBuilder.Length > 0)
                        bodyBuilder.AppendLine();
                    bodyBuilder.Append(line);
                }
            }

            // Flush last message
            if (currentRole != null)
            {
                history.Messages.Add(new ChatMessage
                {
                    Role = currentRole,
                    Text = bodyBuilder.ToString().TrimEnd(),
                    Timestamp = currentTimestamp
                });
            }

            return history;
        }
    }
}
