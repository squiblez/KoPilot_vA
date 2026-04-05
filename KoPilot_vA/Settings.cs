namespace KoPilot_vA
{
    public partial class Settings : Form
    {
        public string EditorFontFamily { get; private set; } = "Consolas";
        public float EditorFontSize { get; private set; } = 11f;
        public bool WordWrap { get; private set; }
        public bool ShowLineNumbers { get; private set; } = true;
        public string DefaultProjectPath { get; private set; } = string.Empty;
        public string DotNetSdkPath { get; private set; } = "dotnet";

        public Settings()
        {
            InitializeComponent();
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            txtFontFamily.Text = EditorFontFamily;
            nudFontSize.Value = (decimal)EditorFontSize;
            chkWordWrap.Checked = WordWrap;
            chkLineNumbers.Checked = ShowLineNumbers;
            txtDefaultPath.Text = DefaultProjectPath;
            txtDotNetPath.Text = DotNetSdkPath;
        }

        public void ApplyFrom(string fontFamily, float fontSize, bool wordWrap, bool lineNumbers, string defaultPath, string dotnetPath)
        {
            EditorFontFamily = fontFamily;
            EditorFontSize = fontSize;
            WordWrap = wordWrap;
            ShowLineNumbers = lineNumbers;
            DefaultProjectPath = defaultPath;
            DotNetSdkPath = dotnetPath;

            LoadCurrentSettings();
        }

        private void btnOk_Click(object? sender, EventArgs e)
        {
            EditorFontFamily = txtFontFamily.Text.Trim();
            EditorFontSize = (float)nudFontSize.Value;
            WordWrap = chkWordWrap.Checked;
            ShowLineNumbers = chkLineNumbers.Checked;
            DefaultProjectPath = txtDefaultPath.Text.Trim();
            DotNetSdkPath = txtDotNetPath.Text.Trim();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnBrowsePath_Click(object? sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog { Description = "Select default project location" };
            if (fbd.ShowDialog() == DialogResult.OK)
                txtDefaultPath.Text = fbd.SelectedPath;
        }
    }
}
