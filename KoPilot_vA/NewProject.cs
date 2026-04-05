namespace KoPilot_vA
{
    public partial class NewProject : Form
    {
        public string ProjectName => txtProjectName.Text.Trim();
        public string ProjectLocation => txtLocation.Text.Trim();
        public string ProjectLanguage => cmbLanguage.SelectedItem?.ToString() ?? "C#";

        public NewProject()
        {
            InitializeComponent();
            cmbLanguage.SelectedIndex = 0;
            txtLocation.Text = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KoPilotProjects");
        }

        private void btnBrowse_Click(object? sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog
            {
                Description = "Select project location",
                SelectedPath = txtLocation.Text
            };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtLocation.Text = fbd.SelectedPath;
            }
        }

        private void btnCreate_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProjectName))
            {
                MessageBox.Show("Please enter a project name.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(ProjectLocation))
            {
                MessageBox.Show("Please select a location.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var fullPath = Path.Combine(ProjectLocation, ProjectName);
            if (Directory.Exists(fullPath) && Directory.GetFileSystemEntries(fullPath).Length > 0)
            {
                var result = MessageBox.Show(
                    "The directory already exists and is not empty. Continue anyway?",
                    "Directory Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
