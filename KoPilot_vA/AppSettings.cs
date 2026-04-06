using System.Text.Json;

namespace KoPilot_vA
{
    /// <summary>
    /// All persistent application settings, serialized to kopilot.cfg as JSON.
    /// </summary>
    internal sealed class AppSettings
    {
        // ===== Editor =====
        public string EditorFontFamily { get; set; } = "Consolas";
        public float EditorFontSize { get; set; } = 11f;
        public bool WordWrap { get; set; }
        public string DotNetPath { get; set; } = "dotnet";

        // ===== AI Endpoint =====
        public string AiEndpointUrl { get; set; } = "http://192.168.20.227:5001";
        public int AiMaxTokens { get; set; } = 4096;
        public int AiContextLength { get; set; } = 250000;
        public double AiTemperature { get; set; } = 0.7;
        public string AiSystemPrompt { get; set; } = string.Empty;
        public bool AiEnabled { get; set; } = true;

        // ===== Recent Projects =====
        public List<string> RecentProjects { get; set; } = new();

        // ===== Config path (next to the application binary for portability) =====
        private static readonly string _cfgPath =
            Path.Combine(AppContext.BaseDirectory, "kopilot.cfg");

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(_cfgPath))
                {
                    var json = File.ReadAllText(_cfgPath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions);
                    if (settings != null)
                        return settings;
                }
            }
            catch { }

            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(this, _jsonOptions);
                File.WriteAllText(_cfgPath, json);
            }
            catch { }
        }
    }
}
