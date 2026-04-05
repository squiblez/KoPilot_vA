using System.Text.Json;
using System.Text.Json.Serialization;

namespace KoPilot_vA
{
    public class KoPilotProject
    {
        public string Name { get; set; } = "Untitled";
        public string RootPath { get; set; } = string.Empty;
        public string ProjectFilePath { get; set; } = string.Empty;
        public List<string> Files { get; set; } = new();
        public string StartupFile { get; set; } = string.Empty;
        public string Language { get; set; } = "C#";
        public string TargetFramework { get; set; } = "net8.0";
        public List<string> OpenFiles { get; set; } = new();
        public string ActiveFile { get; set; } = string.Empty;

        [JsonIgnore]
        public bool IsDirty { get; set; }

        [JsonIgnore]
        public bool IsLoaded => !string.IsNullOrEmpty(ProjectFilePath) && Directory.Exists(RootPath);

        private const string ProjectExtension = ".kopilot";

        public static KoPilotProject Create(string name, string rootPath, string language = "C#")
        {
            var project = new KoPilotProject
            {
                Name = name,
                RootPath = rootPath,
                Language = language,
                ProjectFilePath = Path.Combine(rootPath, name + ProjectExtension)
            };

            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            return project;
        }

        public void AddFile(string relativePath)
        {
            var normalized = relativePath.Replace('\\', '/');
            if (!Files.Contains(normalized))
            {
                Files.Add(normalized);
                IsDirty = true;
            }
        }

        public void RemoveFile(string relativePath)
        {
            var normalized = relativePath.Replace('\\', '/');
            Files.Remove(normalized);
            IsDirty = true;
        }

        public void RenameFile(string oldRelativePath, string newRelativePath)
        {
            var oldNorm = oldRelativePath.Replace('\\', '/');
            var newNorm = newRelativePath.Replace('\\', '/');
            var idx = Files.IndexOf(oldNorm);
            if (idx >= 0)
                Files[idx] = newNorm;
            else
                Files.Add(newNorm);

            if (string.Equals(StartupFile, oldNorm, StringComparison.OrdinalIgnoreCase))
                StartupFile = newNorm;

            IsDirty = true;
        }

        public string GetAbsolutePath(string relativePath)
        {
            return Path.Combine(RootPath, relativePath.Replace('/', '\\'));
        }

        public string GetRelativePath(string absolutePath)
        {
            return Path.GetRelativePath(RootPath, absolutePath).Replace('\\', '/');
        }

        public void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(ProjectFilePath, json);
            IsDirty = false;
        }

        public static KoPilotProject Load(string projectFilePath)
        {
            var json = File.ReadAllText(projectFilePath);
            var project = JsonSerializer.Deserialize<KoPilotProject>(json) ?? new KoPilotProject();
            project.ProjectFilePath = projectFilePath;
            if (string.IsNullOrEmpty(project.RootPath))
                project.RootPath = Path.GetDirectoryName(projectFilePath) ?? string.Empty;
            return project;
        }

        public void ScanDirectory()
        {
            Files.Clear();
            if (!Directory.Exists(RootPath)) return;

            var allFiles = Directory.GetFiles(RootPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                var ext = Path.GetExtension(file).ToLowerInvariant();
                if (ext == ProjectExtension) continue;
                if (file.Contains(Path.Combine(RootPath, "bin"))) continue;
                if (file.Contains(Path.Combine(RootPath, "obj"))) continue;

                var rel = GetRelativePath(file);
                Files.Add(rel);
            }
            IsDirty = true;
        }

        public string GenerateCsproj()
        {
            return $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>{TargetFramework}</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
";
        }
    }
}
