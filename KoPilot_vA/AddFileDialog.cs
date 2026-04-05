namespace KoPilot_vA
{
    public class AddFileDialog : Form
    {
        public string FileName => EnsureExtension(txtFileName.Text.Trim(), FileTemplate);
        public string FileTemplate => cmbTemplate.SelectedItem?.ToString() ?? "Empty File";
        public string Subdirectory => txtSubdirectory.Text.Trim();

        private TextBox txtFileName = null!;
        private TextBox txtSubdirectory = null!;
        private ComboBox cmbTemplate = null!;

        /// <summary>
        /// Returns the expected file extension for each template, or null if any extension is fine.
        /// </summary>
        private static string? GetDefaultExtension(string template) => template switch
        {
            "C# Class"          => ".cs",
            "C# Interface"      => ".cs",
            "C# Program (Main)" => ".cs",
            "JSON File"         => ".json",
            "XML File"          => ".xml",
            "Text File"         => ".txt",
            _                   => null
        };

        /// <summary>
        /// If the file name has no extension, append the correct one for the selected template.
        /// </summary>
        private static string EnsureExtension(string fileName, string template)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return fileName;

            var expected = GetDefaultExtension(template);
            if (expected == null) return fileName;

            var currentExt = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(currentExt))
                return fileName + expected;

            return fileName;
        }

        public AddFileDialog()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = "Add New File";
            Size = new Size(450, 280);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(45, 45, 48);
            ForeColor = Color.White;

            var lblName = new Label { Text = "File Name:", Location = new Point(20, 20), AutoSize = true };
            txtFileName = new TextBox
            {
                Location = new Point(20, 45),
                Size = new Size(390, 25),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                Text = "NewFile.cs"
            };

            var lblSub = new Label { Text = "Subdirectory (optional):", Location = new Point(20, 80), AutoSize = true };
            txtSubdirectory = new TextBox
            {
                Location = new Point(20, 105),
                Size = new Size(390, 25),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White
            };

            var lblTemplate = new Label { Text = "Template:", Location = new Point(20, 140), AutoSize = true };
            cmbTemplate = new ComboBox
            {
                Location = new Point(20, 165),
                Size = new Size(390, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White
            };
            cmbTemplate.Items.AddRange(new object[] {
                "Empty File",
                "C# Class",
                "C# Interface",
                "C# Program (Main)",
                "Text File",
                "JSON File",
                "XML File"
            });
            cmbTemplate.SelectedIndex = 1;

            var btnOk = new Button
            {
                Text = "Add",
                DialogResult = DialogResult.OK,
                Location = new Point(230, 205),
                Size = new Size(85, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(325, 205),
                Size = new Size(85, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(63, 63, 70),
                ForeColor = Color.White
            };

            AcceptButton = btnOk;
            CancelButton = btnCancel;

            Controls.AddRange(new Control[] { lblName, txtFileName, lblSub, txtSubdirectory, lblTemplate, cmbTemplate, btnOk, btnCancel });
        }

        public static string GenerateContent(string template, string fileName, string projectName)
        {
            var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var ns = SanitizeNamespace(projectName);

            return template switch
            {
                "C# Class" => $@"namespace {ns}
{{
    public class {nameWithoutExt}
    {{
        public {nameWithoutExt}()
        {{
        }}
    }}
}}
",
                "C# Interface" => $@"namespace {ns}
{{
    public interface {nameWithoutExt}
    {{
    }}
}}
",
                "C# Program (Main)" => $@"using System;

namespace {ns}
{{
    internal class Program
    {{
        static void Main(string[] args)
        {{
            Console.WriteLine(""Hello, World!"");
        }}
    }}
}}
",
                "JSON File" => "{\n\n}\n",
                "XML File" => "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<root>\n</root>\n",
                _ => string.Empty
            };
        }

        private static string SanitizeNamespace(string name)
        {
            var sanitized = new System.Text.StringBuilder();
            foreach (var c in name)
            {
                if (char.IsLetterOrDigit(c) || c == '_' || c == '.')
                    sanitized.Append(c);
            }
            if (sanitized.Length == 0) return "MyProject";
            if (char.IsDigit(sanitized[0])) sanitized.Insert(0, '_');
            return sanitized.ToString();
        }
    }
}
