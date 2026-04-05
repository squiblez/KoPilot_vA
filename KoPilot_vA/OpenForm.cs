using System.Reflection;
using System.Text.Json;

namespace KoPilot_vA
{
    public partial class OpenForm : Form
    {
        /// <summary>
        /// After the form closes with <see cref="DialogResult.OK"/>, contains
        /// the full path to the .kopilot file the user chose to open,
        /// or <c>null</c> if they chose "New Project".
        /// </summary>
        public string? SelectedProjectPath { get; private set; }

        /// <summary>
        /// <c>true</c> when the user clicked "New Project" instead of opening an existing one.
        /// </summary>
        public bool CreateNewProject { get; private set; }

        private static readonly string _recentProjectsPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "KoPilot", "recent.json");

        private readonly List<string> _recentProjects = new();

        public OpenForm()
        {
            InitializeComponent();
            PopulateAppInfo();
            LoadRecentProjects();
            PopulateRecentList();
        }

        private void PopulateAppInfo()
        {
            var asm = Assembly.GetExecutingAssembly();
            var ver = asm.GetName().Version ?? new Version(1, 0, 0, 0);
            lblVersion.Text = $"Version {ver.Major}.{ver.Minor}.{ver.Build}  \u2022  .NET {Environment.Version}";

            var svc = new KoboldCppService();
            lblEndpoint.Text = $"AI Endpoint: {svc.EndpointUrl}";
        }

        private void LoadRecentProjects()
        {
            _recentProjects.Clear();
            try
            {
                if (File.Exists(_recentProjectsPath))
                {
                    var json = File.ReadAllText(_recentProjectsPath);
                    var list = JsonSerializer.Deserialize<List<string>>(json);
                    if (list != null)
                        _recentProjects.AddRange(list);
                }
            }
            catch { }
        }

        private void PopulateRecentList()
        {
            lstRecent.Items.Clear();

            if (_recentProjects.Count == 0)
            {
                lblNoRecent.Visible = true;
                lstRecent.Visible = false;
                return;
            }

            lblNoRecent.Visible = false;
            lstRecent.Visible = true;

            foreach (var path in _recentProjects)
            {
                var name = Path.GetFileNameWithoutExtension(path);
                var dir = Path.GetDirectoryName(path) ?? string.Empty;
                var item = new ListViewItem(name);
                item.SubItems.Add(dir);
                item.Tag = path;
                item.ForeColor = File.Exists(path)
                    ? Color.FromArgb(220, 220, 220)
                    : Color.FromArgb(100, 100, 100);
                lstRecent.Items.Add(item);
            }

            if (lstRecent.Items.Count > 0)
                lstRecent.Items[0].Selected = true;
        }

        private void BtnOpen_Click(object? sender, EventArgs e)
        {
            OpenSelectedProject();
        }

        private void LstRecent_DoubleClick(object? sender, EventArgs e)
        {
            OpenSelectedProject();
        }

        private void LstRecent_SelectedIndexChanged(object? sender, EventArgs e)
        {
            btnOpen.Enabled = lstRecent.SelectedItems.Count > 0;
        }

        private void OpenSelectedProject()
        {
            if (lstRecent.SelectedItems.Count == 0) return;

            var path = lstRecent.SelectedItems[0].Tag as string;
            if (string.IsNullOrEmpty(path)) return;

            if (!File.Exists(path))
            {
                MessageBox.Show($"Project file not found:\n{path}", "Open Project",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectedProjectPath = path;
            CreateNewProject = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnNew_Click(object? sender, EventArgs e)
        {
            SelectedProjectPath = null;
            CreateNewProject = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnBrowse_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Open KoPilot Project",
                Filter = "KoPilot Project (*.kopilot)|*.kopilot|All Files (*.*)|*.*"
            };

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                SelectedProjectPath = ofd.FileName;
                CreateNewProject = false;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void BtnQuit_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void BtnRemoveRecent_Click(object? sender, EventArgs e)
        {
            if (lstRecent.SelectedItems.Count == 0) return;

            var path = lstRecent.SelectedItems[0].Tag as string;
            if (path != null)
            {
                _recentProjects.RemoveAll(p =>
                    string.Equals(p, path, StringComparison.OrdinalIgnoreCase));
                SaveRecentProjects();
                PopulateRecentList();
            }
        }

        private void SaveRecentProjects()
        {
            try
            {
                var dir = Path.GetDirectoryName(_recentProjectsPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(_recentProjects,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_recentProjectsPath, json);
            }
            catch { }
        }
    }
}
