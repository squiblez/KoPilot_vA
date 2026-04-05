using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace KoPilot_vA
{
    public partial class Form1 : Form
    {
        private KoPilotProject? _currentProject;
        private readonly KoboldCppService _aiService = new();
        private CancellationTokenSource? _buildCts;
        private CancellationTokenSource? _aiCts;
        private Process? _runProcess;
        private ChatHistory _chatHistory = new();

        // Recent projects
        private const int MaxRecentProjects = 10;
        private readonly List<string> _recentProjects = new();
        private static readonly string _recentProjectsPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "KoPilot", "recent.json");

        // Settings state
        private string _editorFontFamily = "Consolas";
        private float _editorFontSize = 11f;
        private bool _wordWrap;
        private string _dotNetPath = "dotnet";

        /// <summary>
        /// When set before the form loads, the project at this path will be
        /// opened automatically on startup (set by <see cref="OpenForm"/>).
        /// </summary>
        public string? ProjectToLoadOnStart { get; set; }

        /// <summary>
        /// When <c>true</c>, the New Project dialog will be shown
        /// automatically on startup (set by <see cref="OpenForm"/>).
        /// </summary>
        public bool RequestNewProjectOnLoad { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates and wires all menu items. Done in code rather than the Designer
        /// so a Designer regeneration can never strip the items.
        /// </summary>
        private void SetupMenus()
        {
            menuStrip.Renderer = new DarkMenuRenderer();

            // ── File ──
            mnuNewProject  = new ToolStripMenuItem("New Project",  null, OnNewProject)  { ShortcutKeys = Keys.Control | Keys.Shift | Keys.N };
            mnuOpenProject = new ToolStripMenuItem("Open Project", null, OnOpenProject) { ShortcutKeys = Keys.Control | Keys.Shift | Keys.O };
            mnuSaveProject = new ToolStripMenuItem("Save Project", null, OnSaveProject);
            mnuCloseProject = new ToolStripMenuItem("Close Project", null, OnCloseProject);

            mnuNewFile   = new ToolStripMenuItem("New File",   null, OnNewFile)   { ShortcutKeys = Keys.Control | Keys.N };
            mnuOpenFile  = new ToolStripMenuItem("Open File",  null, OnOpenFile)  { ShortcutKeys = Keys.Control | Keys.O };
            mnuSaveFile  = new ToolStripMenuItem("Save",       null, OnSaveFile)  { ShortcutKeys = Keys.Control | Keys.S };
            mnuSaveAll   = new ToolStripMenuItem("Save All",   null, OnSaveAll)   { ShortcutKeys = Keys.Control | Keys.Shift | Keys.S };
            mnuCloseFile = new ToolStripMenuItem("Close File",  null, OnCloseFile) { ShortcutKeys = Keys.Control | Keys.W };
            mnuExit      = new ToolStripMenuItem("Exit",       null, OnExit);

            mnuFile.DropDownItems.Clear();
            mnuFile.DropDownItems.AddRange([
                mnuNewProject, mnuOpenProject, mnuSaveProject, mnuCloseProject,
                mnuSep1,
                mnuNewFile, mnuOpenFile, mnuSaveFile, mnuSaveAll, mnuCloseFile,
                mnuSep2,
                mnuRecentProjects,
                mnuSep3,
                mnuExit
            ]);

            // ── Edit ──
            mnuUndo      = new ToolStripMenuItem("Undo",       null, OnUndo)      { ShortcutKeys = Keys.Control | Keys.Z };
            mnuRedo      = new ToolStripMenuItem("Redo",       null, OnRedo)      { ShortcutKeys = Keys.Control | Keys.Y };
            mnuCut       = new ToolStripMenuItem("Cut",        null, OnCut)       { ShortcutKeys = Keys.Control | Keys.X };
            mnuCopy      = new ToolStripMenuItem("Copy",       null, OnCopy)      { ShortcutKeys = Keys.Control | Keys.C };
            mnuPaste     = new ToolStripMenuItem("Paste",      null, OnPaste)     { ShortcutKeys = Keys.Control | Keys.V };
            mnuSelectAll = new ToolStripMenuItem("Select All", null, OnSelectAll) { ShortcutKeys = Keys.Control | Keys.A };
            mnuFind      = new ToolStripMenuItem("Find",       null, OnFind)      { ShortcutKeys = Keys.Control | Keys.F };
            mnuReplace   = new ToolStripMenuItem("Replace",    null, OnReplace)   { ShortcutKeys = Keys.Control | Keys.H };
            mnuGoToLine  = new ToolStripMenuItem("Go To Line", null, OnGoToLine)  { ShortcutKeys = Keys.Control | Keys.G };

            mnuEdit.DropDownItems.Clear();
            mnuEdit.DropDownItems.AddRange([
                mnuUndo, mnuRedo,
                mnuSep4,
                mnuCut, mnuCopy, mnuPaste, mnuSelectAll,
                mnuSep5,
                mnuFind, mnuReplace, mnuGoToLine
            ]);

            // ── Project ──
            mnuAddFile          = new ToolStripMenuItem("Add New File",      null, OnAddFile);
            mnuAddExistingFile  = new ToolStripMenuItem("Add Existing File", null, OnAddExistingFile);
            mnuRemoveFile       = new ToolStripMenuItem("Remove from Project", null, OnRemoveFileFromProject);
            mnuRenameFile       = new ToolStripMenuItem("Rename File",       null, OnRenameFile);
            mnuDeleteFile       = new ToolStripMenuItem("Delete File",       null, OnDeleteFile);
            mnuProjectProperties = new ToolStripMenuItem("Properties",       null, OnProjectProperties);

            mnuProject.DropDownItems.Clear();
            mnuProject.DropDownItems.AddRange([
                mnuAddFile, mnuAddExistingFile,
                mnuSep6,
                mnuRemoveFile, mnuRenameFile, mnuDeleteFile,
                new ToolStripSeparator(),
                mnuProjectProperties
            ]);

            // ── Build ──
            mnuBuildProject   = new ToolStripMenuItem("Build",              null, OnBuildProject)    { ShortcutKeys = Keys.Control | Keys.Shift | Keys.B };
            mnuRun            = new ToolStripMenuItem("Run",                null, OnRunProject)      { ShortcutKeys = Keys.F5 };
            mnuRunWithoutDebug = new ToolStripMenuItem("Run Without Debug", null, OnRunWithoutDebug) { ShortcutKeys = Keys.Control | Keys.F5 };
            mnuClean          = new ToolStripMenuItem("Clean",              null, OnCleanProject);

            mnuBuild.DropDownItems.Clear();
            mnuBuild.DropDownItems.AddRange([
                mnuBuildProject,
                mnuRun,
                mnuRunWithoutDebug,
                new ToolStripSeparator(),
                mnuClean
            ]);

            // ── Tools ──
            mnuSettings   = new ToolStripMenuItem("Settings",    null, OnSettings);
            mnuAISettings = new ToolStripMenuItem("AI Settings", null, OnAISettings);

            mnuTools.DropDownItems.Clear();
            mnuTools.DropDownItems.AddRange([
                mnuSettings,
                mnuAISettings
            ]);

            // ── Window ──
            mnuToggleExplorer = new ToolStripMenuItem("Toggle Explorer",  null, OnToggleExplorer);
            mnuToggleOutput   = new ToolStripMenuItem("Toggle Output",    null, OnToggleOutput);
            mnuToggleAIPanel  = new ToolStripMenuItem("Toggle AI Panel",  null, OnToggleAIPanel);

            mnuWindow.DropDownItems.Clear();
            mnuWindow.DropDownItems.AddRange([
                mnuToggleExplorer,
                mnuToggleOutput,
                mnuToggleAIPanel
            ]);

            // ── Help ──
            mnuAbout = new ToolStripMenuItem("About", null, OnAbout);

            mnuHelp.DropDownItems.Clear();
            mnuHelp.DropDownItems.AddRange([
                mnuAbout
            ]);
        }

        // ===== PROJECT MANAGEMENT =====

        private void OnNewProject(object? sender, EventArgs e)
        {
            if (!ConfirmCloseCurrentProject()) return;

            using var dlg = new NewProject();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                var projectDir = Path.Combine(dlg.ProjectLocation, dlg.ProjectName);
                _currentProject = KoPilotProject.Create(dlg.ProjectName, projectDir, dlg.ProjectLanguage);

                // Create default Program.cs
                var programFile = "Program.cs";
                var programPath = _currentProject.GetAbsolutePath(programFile);
                var content = AddFileDialog.GenerateContent("C# Program (Main)", programFile, _currentProject.Name);
                File.WriteAllText(programPath, content);
                _currentProject.AddFile(programFile);

                // Create .csproj
                var csprojPath = Path.Combine(projectDir, _currentProject.Name + ".csproj");
                File.WriteAllText(csprojPath, _currentProject.GenerateCsproj());

                _currentProject.StartupFile = programFile;
                _currentProject.Save();

                AddToRecentProjects(_currentProject.ProjectFilePath);
                _chatHistory.Clear();
                txtAIChat.Clear();
                RefreshExplorer();
                UpdateTitle();
                AppendOutput($"Project '{_currentProject.Name}' created at {projectDir}");
                SetStatus("Project created");
            }
        }

        private void OnOpenProject(object? sender, EventArgs e)
        {
            if (!ConfirmCloseCurrentProject()) return;

            using var ofd = new OpenFileDialog
            {
                Title = "Open KoPilot Project",
                Filter = "KoPilot Project (*.kopilot)|*.kopilot|All Files (*.*)|*.*"
            };

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                LoadProject(ofd.FileName);
            }
        }

        private void LoadProject(string path)
        {
            try
            {
                CloseAllTabs();
                _currentProject = KoPilotProject.Load(path);
                RefreshExplorer();
                UpdateTitle();
                AppendOutput($"Project '{_currentProject.Name}' loaded from {path}");
                SetStatus("Project loaded");

                LoadChatHistory();
                AddToRecentProjects(path);
                RestoreOpenFilesState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load project: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnSaveProject(object? sender, EventArgs e)
        {
            if (_currentProject == null) return;
            SaveOpenFilesState();
            _currentProject.Save();
            SetStatus("Project saved");
        }

        private void OnCloseProject(object? sender, EventArgs e)
        {
            if (!ConfirmCloseCurrentProject()) return;
            SaveChatHistory();
            CloseAllTabs();
            _currentProject = null;
            _chatHistory.Clear();
            txtAIChat.Clear();
            RefreshExplorer();
            UpdateTitle();
            SetStatus("Project closed");
        }

        private bool ConfirmCloseCurrentProject()
        {
            if (_currentProject == null) return true;

            // Save which files are currently open before closing
            SaveOpenFilesState();
            _currentProject.Save();

            // Check for unsaved editor tabs
            foreach (TabPage tab in tabEditor.TabPages)
            {
                if (tab is EditorTabPage etp && etp.IsModified)
                {
                    var result = MessageBox.Show(
                        "There are unsaved changes. Save all before closing?",
                        "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel) return false;
                    if (result == DialogResult.Yes) SaveAllFiles();
                    break;
                }
            }
            return true;
        }

        // ===== FILE MANAGEMENT =====

        private void OnNewFile(object? sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Title = "Create New File",
                Filter = "C# Files (*.cs)|*.cs|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                InitialDirectory = _currentProject?.RootPath ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, string.Empty);
                if (_currentProject != null)
                {
                    var rel = _currentProject.GetRelativePath(sfd.FileName);
                    _currentProject.AddFile(rel);
                    _currentProject.Save();
                    RefreshExplorer();
                }
                OpenFileInEditor(sfd.FileName);
            }
        }

        private void OnOpenFile(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Open File",
                Filter = "C# Files (*.cs)|*.cs|All Files (*.*)|*.*",
                InitialDirectory = _currentProject?.RootPath ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                OpenFileInEditor(ofd.FileName);
            }
        }

        private void OnSaveFile(object? sender, EventArgs e)
        {
            if (tabEditor.SelectedTab is EditorTabPage etp)
            {
                etp.SaveContent();
                SetStatus($"Saved {Path.GetFileName(etp.FilePath)}");
            }
        }

        private void OnSaveAll(object? sender, EventArgs e)
        {
            SaveAllFiles();
            SetStatus("All files saved");
        }

        private void SaveAllFiles()
        {
            foreach (TabPage tab in tabEditor.TabPages)
            {
                if (tab is EditorTabPage etp && etp.IsModified)
                    etp.SaveContent();
            }
            _currentProject?.Save();
        }

        private void OnCloseFile(object? sender, EventArgs e)
        {
            if (tabEditor.SelectedTab is EditorTabPage etp)
            {
                if (etp.IsModified)
                {
                    var result = MessageBox.Show(
                        $"Save changes to '{Path.GetFileName(etp.FilePath)}'?",
                        "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel) return;
                    if (result == DialogResult.Yes) etp.SaveContent();
                }
                tabEditor.TabPages.Remove(etp);
                etp.Dispose();
            }
        }

        private void CloseAllTabs()
        {
            var tabs = new List<TabPage>();
            foreach (TabPage tab in tabEditor.TabPages)
                tabs.Add(tab);
            tabEditor.TabPages.Clear();
            foreach (var tab in tabs)
                tab.Dispose();
        }

        private void OnAddFile(object? sender, EventArgs e)
        {
            if (_currentProject == null)
            {
                MessageBox.Show("No project is open.", "Add File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dlg = new AddFileDialog();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                var relPath = string.IsNullOrEmpty(dlg.Subdirectory)
                    ? dlg.FileName
                    : Path.Combine(dlg.Subdirectory, dlg.FileName);

                var absPath = _currentProject.GetAbsolutePath(relPath);
                if (File.Exists(absPath))
                {
                    MessageBox.Show("File already exists.", "Add File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var dir = Path.GetDirectoryName(absPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var content = AddFileDialog.GenerateContent(dlg.FileTemplate, dlg.FileName, _currentProject.Name);
                File.WriteAllText(absPath, content);
                _currentProject.AddFile(relPath.Replace('\\', '/'));
                _currentProject.Save();
                RefreshExplorer();
                OpenFileInEditor(absPath);
                SetStatus($"Added {dlg.FileName}");
            }
        }

        private void OnAddExistingFile(object? sender, EventArgs e)
        {
            if (_currentProject == null) return;

            using var ofd = new OpenFileDialog
            {
                Title = "Add Existing File",
                Filter = "All Files (*.*)|*.*",
                Multiselect = true,
                InitialDirectory = _currentProject.RootPath
            };

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                foreach (var file in ofd.FileNames)
                {
                    string targetPath;
                    if (file.StartsWith(_currentProject.RootPath, StringComparison.OrdinalIgnoreCase))
                    {
                        targetPath = file;
                    }
                    else
                    {
                        targetPath = Path.Combine(_currentProject.RootPath, Path.GetFileName(file));
                        if (!File.Exists(targetPath))
                            File.Copy(file, targetPath);
                    }

                    var rel = _currentProject.GetRelativePath(targetPath);
                    _currentProject.AddFile(rel);
                }
                _currentProject.Save();
                RefreshExplorer();
                SetStatus("Existing file(s) added");
            }
        }

        private void OnRemoveFileFromProject(object? sender, EventArgs e)
        {
            if (_currentProject == null) return;
            var node = treeFiles.SelectedNode;
            if (node?.Tag is string relPath)
            {
                _currentProject.RemoveFile(relPath);
                _currentProject.Save();
                RefreshExplorer();
                SetStatus($"Removed {relPath} from project");
            }
        }

        private void OnDeleteFile(object? sender, EventArgs e)
        {
            if (_currentProject == null) return;
            var node = treeFiles.SelectedNode;
            if (node?.Tag is string relPath)
            {
                var result = MessageBox.Show(
                    $"Permanently delete '{relPath}'?", "Delete File",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    var absPath = _currentProject.GetAbsolutePath(relPath);

                    // Close tab if open
                    CloseTabForFile(absPath);

                    if (File.Exists(absPath))
                        File.Delete(absPath);
                    _currentProject.RemoveFile(relPath);
                    _currentProject.Save();
                    RefreshExplorer();
                    SetStatus($"Deleted {relPath}");
                }
            }
        }

        private void OnRenameFile(object? sender, EventArgs e)
        {
            if (_currentProject == null) return;
            var node = treeFiles.SelectedNode;
            if (node?.Tag is not string oldRelPath) return;

            var oldFileName = Path.GetFileName(oldRelPath);
            var newFileName = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter new file name:", "Rename File", oldFileName);

            if (string.IsNullOrWhiteSpace(newFileName) || newFileName == oldFileName) return;

            // Build the new relative path keeping the same directory
            var dir = Path.GetDirectoryName(oldRelPath.Replace('/', '\\'));
            var newRelPath = string.IsNullOrEmpty(dir)
                ? newFileName
                : Path.Combine(dir, newFileName).Replace('\\', '/');

            var oldAbsPath = _currentProject.GetAbsolutePath(oldRelPath);
            var newAbsPath = _currentProject.GetAbsolutePath(newRelPath);

            if (File.Exists(newAbsPath))
            {
                MessageBox.Show($"A file named '{newFileName}' already exists in that location.",
                    "Rename File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Rename on disk
                File.Move(oldAbsPath, newAbsPath);

                // Update project file list
                _currentProject.RenameFile(oldRelPath, newRelPath);
                _currentProject.Save();

                // Update any open editor tab pointing to the old path
                foreach (TabPage tab in tabEditor.TabPages)
                {
                    if (tab is EditorTabPage etp &&
                        string.Equals(etp.FilePath, oldAbsPath, StringComparison.OrdinalIgnoreCase))
                    {
                        etp.FilePath = newAbsPath;
                        etp.Tag = newAbsPath;
                        etp.Text = Path.GetFileName(newAbsPath);
                        break;
                    }
                }

                RefreshExplorer();
                SetStatus($"Renamed '{oldFileName}' to '{newFileName}'");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to rename file: {ex.Message}", "Rename Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseTabForFile(string absolutePath)
        {
            EditorTabPage? toRemove = null;
            foreach (TabPage tab in tabEditor.TabPages)
            {
                if (tab is EditorTabPage etp &&
                    string.Equals(etp.FilePath, absolutePath, StringComparison.OrdinalIgnoreCase))
                {
                    toRemove = etp;
                    break;
                }
            }
            if (toRemove != null)
            {
                tabEditor.TabPages.Remove(toRemove);
                toRemove.Dispose();
            }
        }

        /// <summary>
        /// Records the currently open editor tabs and the active tab
        /// into the project model so they can be restored later.
        /// </summary>
        private void SaveOpenFilesState()
        {
            if (_currentProject == null) return;

            _currentProject.OpenFiles.Clear();
            foreach (TabPage tab in tabEditor.TabPages)
            {
                if (tab is EditorTabPage etp)
                {
                    var rel = _currentProject.GetRelativePath(etp.FilePath);
                    _currentProject.OpenFiles.Add(rel);
                }
            }

            _currentProject.ActiveFile = string.Empty;
            if (tabEditor.SelectedTab is EditorTabPage active)
                _currentProject.ActiveFile = _currentProject.GetRelativePath(active.FilePath);
        }

        /// <summary>
        /// Reopens editor tabs that were open when the project was last saved.
        /// </summary>
        private void RestoreOpenFilesState()
        {
            if (_currentProject == null) return;

            EditorTabPage? activeTab = null;
            foreach (var rel in _currentProject.OpenFiles)
            {
                var absPath = _currentProject.GetAbsolutePath(rel);
                if (!File.Exists(absPath)) continue;

                OpenFileInEditor(absPath);

                if (string.Equals(rel, _currentProject.ActiveFile, StringComparison.OrdinalIgnoreCase))
                {
                    // Find the tab we just opened
                    foreach (TabPage tab in tabEditor.TabPages)
                    {
                        if (tab is EditorTabPage etp &&
                            string.Equals(etp.FilePath, absPath, StringComparison.OrdinalIgnoreCase))
                        {
                            activeTab = etp;
                            break;
                        }
                    }
                }
            }

            if (activeTab != null)
                tabEditor.SelectedTab = activeTab;
        }

        private void OnProjectProperties(object? sender, EventArgs e)
        {
            if (_currentProject == null)
            {
                MessageBox.Show("No project is open.", "Project Properties", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show(
                $"Project: {_currentProject.Name}\n" +
                $"Path: {_currentProject.RootPath}\n" +
                $"Language: {_currentProject.Language}\n" +
                $"Framework: {_currentProject.TargetFramework}\n" +
                $"Files: {_currentProject.Files.Count}\n" +
                $"Startup: {_currentProject.StartupFile}",
                "Project Properties", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ===== EDITOR =====

        private void OpenFileInEditor(string absolutePath)
        {
            // Check if already open
            foreach (TabPage tab in tabEditor.TabPages)
            {
                if (tab is EditorTabPage etp &&
                    string.Equals(etp.FilePath, absolutePath, StringComparison.OrdinalIgnoreCase))
                {
                    tabEditor.SelectedTab = etp;
                    return;
                }
            }

            var newTab = new EditorTabPage(absolutePath);
            newTab.LoadContent();
            newTab.Editor.SelectionChanged += Editor_SelectionChanged;
            newTab.Editor.TextChanged += EditorSibling_TextChanged;
            tabEditor.TabPages.Add(newTab);
            tabEditor.SelectedTab = newTab;

            // Feed all other project .cs files into this tab's Roslyn workspace
            LoadProjectSiblingsIntoTab(newTab);
        }

        /// <summary>
        /// Reads every .cs file in the current project (except the tab's own file)
        /// and registers it as a sibling document in that tab's Roslyn workspace,
        /// so cross-file types and members are visible to IntelliSense.
        /// </summary>
        private void LoadProjectSiblingsIntoTab(EditorTabPage tab)
        {
            if (_currentProject == null || tab.RoslynService == null) return;

            foreach (var relPath in _currentProject.Files)
            {
                if (!relPath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) continue;

                var absPath = _currentProject.GetAbsolutePath(relPath);
                if (string.Equals(absPath, tab.FilePath, StringComparison.OrdinalIgnoreCase)) continue;
                if (!File.Exists(absPath)) continue;

                // Prefer the live editor text if the file is already open in a tab
                var text = GetLiveTextForFile(absPath);
                tab.RoslynService.AddOrUpdateSiblingDocument(absPath, text);
            }
        }

        /// <summary>
        /// Returns the current in-editor text for a file if it is open in a tab,
        /// otherwise reads it from disk.
        /// </summary>
        private string GetLiveTextForFile(string absolutePath)
        {
            foreach (TabPage tab in tabEditor.TabPages)
            {
                if (tab is EditorTabPage etp &&
                    string.Equals(etp.FilePath, absolutePath, StringComparison.OrdinalIgnoreCase))
                    return etp.Editor.Text;
            }
            return File.ReadAllText(absolutePath);
        }

        /// <summary>
        /// When any editor's text changes, propagate the updated text to every
        /// other open tab's Roslyn workspace so cross-file completions stay fresh.
        /// </summary>
        private void EditorSibling_TextChanged(object? sender, EventArgs e)
        {
            if (sender is not RichTextBox rtb) return;

            // Find which tab owns this RichTextBox
            EditorTabPage? sourceTab = null;
            foreach (TabPage tab in tabEditor.TabPages)
            {
                if (tab is EditorTabPage etp && etp.Editor == rtb)
                {
                    sourceTab = etp;
                    break;
                }
            }
            if (sourceTab == null) return;

            var changedPath = sourceTab.FilePath;
            var changedText = rtb.Text;

            // Push the updated text into every other open tab's sibling registry
            foreach (TabPage tab in tabEditor.TabPages)
            {
                if (tab is EditorTabPage etp &&
                    etp != sourceTab &&
                    etp.RoslynService != null)
                {
                    etp.RoslynService.AddOrUpdateSiblingDocument(changedPath, changedText);
                }
            }
        }

        private void TabEditor_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateLineCol();
        }

        private void TabEditor_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                for (int i = 0; i < tabEditor.TabCount; i++)
                {
                    if (tabEditor.GetTabRect(i).Contains(e.Location))
                    {
                        if (tabEditor.TabPages[i] is EditorTabPage etp)
                        {
                            if (etp.IsModified)
                            {
                                var result = MessageBox.Show(
                                    $"Save changes to '{Path.GetFileName(etp.FilePath)}'?",
                                    "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                                if (result == DialogResult.Cancel) return;
                                if (result == DialogResult.Yes) etp.SaveContent();
                            }
                            tabEditor.TabPages.RemoveAt(i);
                            etp.Dispose();
                        }
                        break;
                    }
                }
            }
        }

        private void TabEditor_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            for (int i = 0; i < tabEditor.TabCount; i++)
            {
                if (!tabEditor.GetTabRect(i).Contains(e.Location)) continue;

                tabEditor.SelectedIndex = i;
                var cms = new ContextMenuStrip();
                cms.BackColor = Color.FromArgb(45, 45, 48);
                cms.ForeColor = Color.White;
                cms.Renderer = new DarkMenuRenderer();

                var closeItem = new ToolStripMenuItem("Close");
                closeItem.Click += (_, _) => OnCloseFile(null, EventArgs.Empty);

                var closeAllItem = new ToolStripMenuItem("Close All");
                closeAllItem.Click += (_, _) =>
                {
                    if (!ConfirmCloseAllTabs()) return;
                    CloseAllTabs();
                };

                var closeOthersItem = new ToolStripMenuItem("Close Others");
                closeOthersItem.Click += (_, _) =>
                {
                    var keep = tabEditor.SelectedTab;
                    var toClose = new List<TabPage>();
                    foreach (TabPage tab in tabEditor.TabPages)
                    {
                        if (tab != keep) toClose.Add(tab);
                    }

                    // Check for unsaved changes in tabs being closed
                    bool hasUnsaved = toClose.Any(t => t is EditorTabPage et && et.IsModified);
                    if (hasUnsaved)
                    {
                        var result = MessageBox.Show(
                            "Some files have unsaved changes. Save all before closing?",
                            "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if (result == DialogResult.Cancel) return;
                        if (result == DialogResult.Yes)
                        {
                            foreach (var t in toClose)
                                if (t is EditorTabPage et && et.IsModified) et.SaveContent();
                        }
                    }

                    foreach (var t in toClose)
                    {
                        tabEditor.TabPages.Remove(t);
                        t.Dispose();
                    }
                };

                cms.Items.Add(closeItem);
                cms.Items.Add(closeOthersItem);
                cms.Items.Add(closeAllItem);
                cms.Show(tabEditor, e.Location);
                break;
            }
        }

        private bool ConfirmCloseAllTabs()
        {
            foreach (TabPage tab in tabEditor.TabPages)
            {
                if (tab is EditorTabPage etp && etp.IsModified)
                {
                    var result = MessageBox.Show(
                        "There are unsaved changes. Save all before closing?",
                        "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel) return false;
                    if (result == DialogResult.Yes) SaveAllFiles();
                    break;
                }
            }
            return true;
        }

        private void Editor_SelectionChanged(object? sender, EventArgs e) => UpdateLineCol();

        private void UpdateLineCol()
        {
            if (tabEditor.SelectedTab is EditorTabPage etp)
            {
                var pos = etp.Editor.SelectionStart;
                var line = etp.Editor.GetLineFromCharIndex(pos) + 1;
                var firstChar = etp.Editor.GetFirstCharIndexOfCurrentLine();
                var col = pos - firstChar + 1;
                lblLineCol.Text = $"Ln {line}, Col {col}";
            }
            else
            {
                lblLineCol.Text = "Ln 1, Col 1";
            }
        }

        // ===== EDIT OPERATIONS =====

        private void OnUndo(object? sender, EventArgs e)
        {
            if (tabEditor.SelectedTab is EditorTabPage etp) etp.Editor.Undo();
        }

        private void OnRedo(object? sender, EventArgs e)
        {
            if (tabEditor.SelectedTab is EditorTabPage etp) etp.Editor.Redo();
        }

        private void OnCut(object? sender, EventArgs e)
        {
            if (tabEditor.SelectedTab is EditorTabPage etp) etp.Editor.Cut();
        }

        private void OnCopy(object? sender, EventArgs e)
        {
            if (tabEditor.SelectedTab is EditorTabPage etp) etp.Editor.Copy();
        }

        private void OnPaste(object? sender, EventArgs e)
        {
            if (tabEditor.SelectedTab is EditorTabPage etp) etp.Editor.Paste();
        }

        private void OnSelectAll(object? sender, EventArgs e)
        {
            if (tabEditor.SelectedTab is EditorTabPage etp) etp.Editor.SelectAll();
        }

        private void OnFind(object? sender, EventArgs e)
        {
            if (tabEditor.SelectedTab is not EditorTabPage etp) return;

            var findText = Microsoft.VisualBasic.Interaction.InputBox(
                "Find:", "Find", string.Empty);
            if (string.IsNullOrEmpty(findText)) return;

            var startPos = etp.Editor.SelectionStart + etp.Editor.SelectionLength;
            var index = etp.Editor.Text.IndexOf(findText, startPos, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                index = etp.Editor.Text.IndexOf(findText, 0, StringComparison.OrdinalIgnoreCase);

            if (index >= 0)
            {
                etp.Editor.Select(index, findText.Length);
                etp.Editor.ScrollToCaret();
                etp.Editor.Focus();
            }
            else
            {
                MessageBox.Show("Text not found.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnReplace(object? sender, EventArgs e)
        {
            if (tabEditor.SelectedTab is not EditorTabPage etp) return;

            var findText = Microsoft.VisualBasic.Interaction.InputBox("Find:", "Replace", string.Empty);
            if (string.IsNullOrEmpty(findText)) return;
            var replaceText = Microsoft.VisualBasic.Interaction.InputBox("Replace with:", "Replace", string.Empty);

            int count = 0;
            int index = 0;
            while ((index = etp.Editor.Text.IndexOf(findText, index, StringComparison.OrdinalIgnoreCase)) >= 0)
            {
                etp.Editor.Select(index, findText.Length);
                etp.Editor.SelectedText = replaceText;
                index += replaceText.Length;
                count++;
            }
            MessageBox.Show($"Replaced {count} occurrence(s).", "Replace", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnGoToLine(object? sender, EventArgs e)
        {
            if (tabEditor.SelectedTab is not EditorTabPage etp) return;

            var input = Microsoft.VisualBasic.Interaction.InputBox("Go to line:", "Go To Line", "1");
            if (int.TryParse(input, out int lineNum) && lineNum >= 1)
            {
                var lines = etp.Editor.Lines;
                if (lineNum > lines.Length) lineNum = lines.Length;
                var charIndex = etp.Editor.GetFirstCharIndexFromLine(lineNum - 1);
                if (charIndex >= 0)
                {
                    etp.Editor.SelectionStart = charIndex;
                    etp.Editor.SelectionLength = 0;
                    etp.Editor.ScrollToCaret();
                    etp.Editor.Focus();
                }
            }
        }

        // ===== BUILD / RUN =====

        private async void OnBuildProject(object? sender, EventArgs e)
        {
            if (_currentProject == null)
            {
                AppendOutput("No project open.");
                return;
            }

            SaveAllFiles();
            lvErrors.Items.Clear();
            AppendOutput("--- Build started ---");
            SetStatus("Building...");

            _buildCts?.Cancel();
            _buildCts = new CancellationTokenSource();

            try
            {
                var csprojPath = Path.Combine(_currentProject.RootPath, _currentProject.Name + ".csproj");
                if (!File.Exists(csprojPath))
                {
                    AppendOutput("No .csproj found. Generating...");
                    File.WriteAllText(csprojPath, _currentProject.GenerateCsproj());
                }

                var output = await RunDotNetCommandAsync($"build \"{csprojPath}\"", _buildCts.Token);
                AppendOutput(output);

                ParseBuildErrors(output);

                if (output.Contains("Build succeeded", StringComparison.OrdinalIgnoreCase))
                    SetStatus("Build succeeded");
                else
                {
                    SetStatus($"Build failed — {lvErrors.Items.Count} error(s)");
                    if (lvErrors.Items.Count > 0)
                    {
                        if (splitRight.Panel2Collapsed)
                            splitRight.Panel2Collapsed = false;
                        tabBottom.SelectedTab = tabErrors;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                AppendOutput("Build cancelled.");
                SetStatus("Build cancelled");
            }
        }

        private async void OnRunProject(object? sender, EventArgs e)
        {
            if (_runProcess != null && !_runProcess.HasExited)
            {
                // Already running — stop it
                try { _runProcess.Kill(entireProcessTree: true); } catch { }
                return;
            }

            if (_currentProject == null)
            {
                AppendOutput("No project open.");
                return;
            }

            SaveAllFiles();

            var csprojPath = Path.Combine(_currentProject.RootPath, _currentProject.Name + ".csproj");
            if (!File.Exists(csprojPath))
            {
                AppendOutput("No .csproj found. Please build the project first.");
                return;
            }

            AppendOutput("--- Run started (external console) ---");
            SetStatus("Running...");
            SetRunButtonState(running: true);

            try
            {
                _runProcess?.Dispose();

                // Launch dotnet directly with CreateNoWindow=false.
                // On Windows this allocates a new console window for the child process,
                // so the user gets full console I/O. We redirect only stderr so we can
                // parse runtime exceptions when the process exits.
                _runProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _dotNetPath,
                        Arguments = $"run --project \"{csprojPath}\"",
                        WorkingDirectory = _currentProject.RootPath,
                        UseShellExecute = false,
                        CreateNoWindow = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = false
                    },
                    EnableRaisingEvents = true
                };

                var stderrBuilder = new System.Text.StringBuilder();

                _runProcess.ErrorDataReceived += (_, args) =>
                {
                    if (args.Data != null)
                        stderrBuilder.AppendLine(args.Data);
                };

                _runProcess.Exited += (_, _) =>
                {
                    var exitCode = -1;
                    try { exitCode = _runProcess.ExitCode; } catch { }
                    var stderr = stderrBuilder.ToString();

                    if (InvokeRequired)
                        Invoke(() => OnRunProcessExited(exitCode, stderr));
                    else
                        OnRunProcessExited(exitCode, stderr);
                };

                _runProcess.Start();
                _runProcess.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                AppendOutput($"Run error: {ex.Message}");
                SetStatus("Run failed");
                SetRunButtonState(running: false);
            }
        }

        private void OnRunProcessExited(int exitCode, string stderrOutput)
        {
            SetRunButtonState(running: false);

            if (!string.IsNullOrWhiteSpace(stderrOutput))
            {
                AppendOutput("--- stderr output ---");
                AppendOutput(stderrOutput.TrimEnd());
            }

            if (exitCode != 0)
            {
                AppendOutput($"--- Process exited with code {exitCode} ---");
                SetStatus($"Run failed (exit code {exitCode})");

                // Show output panel so the user sees the error
                if (splitRight.Panel2Collapsed)
                    splitRight.Panel2Collapsed = false;
                tabBottom.SelectedTab = tabOutput;

                // Try to parse and navigate to the exception location
                var (filePath, line) = ParseRuntimeException(stderrOutput);
                if (filePath != null && line > 0)
                {
                    NavigateToFileAndLine(filePath, line);
                }
            }
            else
            {
                AppendOutput("--- Run completed ---");
                SetStatus("Run completed");
            }
        }

        private void SetRunButtonState(bool running)
        {
            if (InvokeRequired)
            {
                Invoke(() => SetRunButtonState(running));
                return;
            }

            if (running)
            {
                tsbRun.Text = "Stop";
                tsbRun.ToolTipText = "Stop Running Process";
                if (mnuRun != null) mnuRun.Text = "Stop";
            }
            else
            {
                tsbRun.Text = "\u25B6 Run";
                tsbRun.ToolTipText = "Run Project";
                if (mnuRun != null) mnuRun.Text = "Run";
            }
        }

        /// <summary>
        /// Parses a .NET runtime exception stack trace from stderr output.
        /// Matches lines like:
        ///   at Namespace.Class.Method() in C:\path\File.cs:line 12
        /// Returns the first file path and line number found, or (null, 0).
        /// </summary>
        private (string? filePath, int line) ParseRuntimeException(string stderrOutput)
        {
            if (string.IsNullOrEmpty(stderrOutput)) return (null, 0);

            // .NET stack trace format: "at ... in <filepath>:line <number>"
            var regex = new Regex(
                @"in\s+(.+?):line\s+(\d+)",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach (Match match in regex.Matches(stderrOutput))
            {
                var filePath = match.Groups[1].Value.Trim();
                if (int.TryParse(match.Groups[2].Value, out int lineNum) && File.Exists(filePath))
                {
                    return (filePath, lineNum);
                }
            }

            return (null, 0);
        }

        private void OnRunWithoutDebug(object? sender, EventArgs e)
        {
            if (_currentProject == null)
            {
                AppendOutput("No project open.");
                return;
            }

            SaveAllFiles();

            var csprojPath = Path.Combine(_currentProject.RootPath, _currentProject.Name + ".csproj");
            if (!File.Exists(csprojPath))
            {
                AppendOutput("No .csproj found. Please build the project first.");
                return;
            }

            AppendOutput("--- Launching project in console window ---");
            SetStatus("Running...");

            try
            {
                var runArgs = $"/K \"\"{_dotNetPath}\" run --project \"{csprojPath}\"\"";
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = runArgs,
                    WorkingDirectory = _currentProject.RootPath,
                    UseShellExecute = true,
                    CreateNoWindow = false
                });
                SetStatus("Running in external console");
            }
            catch (Exception ex)
            {
                AppendOutput($"Run error: {ex.Message}");
                SetStatus("Run failed");
            }
        }

        private async void OnCleanProject(object? sender, EventArgs e)
        {
            if (_currentProject == null) return;

            var csprojPath = Path.Combine(_currentProject.RootPath, _currentProject.Name + ".csproj");
            if (!File.Exists(csprojPath))
            {
                AppendOutput("No .csproj found to clean.");
                return;
            }

            AppendOutput("--- Cleaning ---");
            var output = await RunDotNetCommandAsync($"clean \"{csprojPath}\"", CancellationToken.None);
            AppendOutput(output);
            SetStatus("Clean completed");
        }

        private async Task<string> RunDotNetCommandAsync(string arguments, CancellationToken ct)
        {
            var psi = new ProcessStartInfo
            {
                FileName = _dotNetPath,
                Arguments = arguments,
                WorkingDirectory = _currentProject?.RootPath ?? Environment.CurrentDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };
            process.Start();

            var stdOut = await process.StandardOutput.ReadToEndAsync(ct);
            var stdErr = await process.StandardError.ReadToEndAsync(ct);
            await process.WaitForExitAsync(ct);

            return string.IsNullOrWhiteSpace(stdErr) ? stdOut : stdOut + "\n" + stdErr;
        }

        // ===== TOOLS / SETTINGS =====

        private void OnSettings(object? sender, EventArgs e)
        {
            using var dlg = new Settings();
            dlg.ApplyFrom(_editorFontFamily, _editorFontSize, _wordWrap, true,
                string.Empty, _dotNetPath);

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _editorFontFamily = dlg.EditorFontFamily;
                _editorFontSize = dlg.EditorFontSize;
                _wordWrap = dlg.WordWrap;
                _dotNetPath = dlg.DotNetSdkPath;

                ApplyEditorSettings();
                SetStatus("Settings applied");
            }
        }

        private void ApplyEditorSettings()
        {
            var font = new Font(_editorFontFamily, _editorFontSize);
            foreach (TabPage tab in tabEditor.TabPages)
            {
                if (tab is EditorTabPage etp)
                {
                    etp.Editor.Font = font;
                    etp.Editor.WordWrap = _wordWrap;
                }
            }
        }

        private void OnAISettings(object? sender, EventArgs e)
        {
            using var dlg = new AIEndPointSettings();
            dlg.ApplyFrom(_aiService.EndpointUrl, _aiService.MaxTokens,
                _aiService.ContextLength, _aiService.Temperature,
                _aiService.SystemPrompt, _aiService.IsEnabled);

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _aiService.EndpointUrl = dlg.EndpointUrl;
                _aiService.MaxTokens = dlg.MaxTokens;
                _aiService.ContextLength = dlg.ContextLength;
                _aiService.Temperature = dlg.Temperature;
                _aiService.SystemPrompt = dlg.SystemPrompt;
                _aiService.IsEnabled = dlg.IsEnabled;
                SetStatus("AI settings updated");
            }
        }

        // ===== WINDOW LAYOUT =====

        private void OnToggleExplorer(object? sender, EventArgs e)
        {
            splitMain.Panel1Collapsed = !splitMain.Panel1Collapsed;
        }

        private void OnToggleOutput(object? sender, EventArgs e)
        {
            splitRight.Panel2Collapsed = !splitRight.Panel2Collapsed;
        }

        private void OnToggleAIPanel(object? sender, EventArgs e)
        {
            splitCenter.Panel2Collapsed = !splitCenter.Panel2Collapsed;
        }

        // ===== HELP =====

        private void OnAbout(object? sender, EventArgs e)
        {
            using var dlg = new About();
            dlg.ShowDialog(this);
        }

        private void OnExit(object? sender, EventArgs e)
        {
            Close();
        }

        // ===== TREE VIEW =====

        private void RefreshExplorer()
        {
            treeFiles.Nodes.Clear();

            if (_currentProject == null)
            {
                lblProjectName.Text = "No Project";
                return;
            }

            lblProjectName.Text = _currentProject.Name;
            var rootNode = new TreeNode(_currentProject.Name)
            {
                ForeColor = Color.FromArgb(220, 220, 220)
            };

            // Build tree from file list
            foreach (var relFile in _currentProject.Files.OrderBy(f => f))
            {
                var parts = relFile.Split('/');
                var parent = rootNode;

                for (int i = 0; i < parts.Length; i++)
                {
                    if (i == parts.Length - 1)
                    {
                        // File node
                        var fileNode = new TreeNode(parts[i])
                        {
                            Tag = relFile,
                            ForeColor = Color.FromArgb(220, 220, 220)
                        };
                        parent.Nodes.Add(fileNode);
                    }
                    else
                    {
                        // Folder node
                        var existing = parent.Nodes.Cast<TreeNode>()
                            .FirstOrDefault(n => n.Text == parts[i] && n.Tag == null);
                        if (existing != null)
                        {
                            parent = existing;
                        }
                        else
                        {
                            var folderNode = new TreeNode(parts[i])
                            {
                                ForeColor = Color.FromArgb(170, 170, 170)
                            };
                            parent.Nodes.Add(folderNode);
                            parent = folderNode;
                        }
                    }
                }
            }

            treeFiles.Nodes.Add(rootNode);
            rootNode.Expand();
        }

        private void TreeFiles_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node?.Tag is string relPath && _currentProject != null)
            {
                var absPath = _currentProject.GetAbsolutePath(relPath);
                if (File.Exists(absPath))
                    OpenFileInEditor(absPath);
            }
        }

        private void OnTreeOpenFile(object? sender, EventArgs e)
        {
            if (treeFiles.SelectedNode?.Tag is string relPath && _currentProject != null)
            {
                var absPath = _currentProject.GetAbsolutePath(relPath);
                if (File.Exists(absPath))
                    OpenFileInEditor(absPath);
            }
        }

        private void OnRefreshTree(object? sender, EventArgs e)
        {
            if (_currentProject != null)
            {
                _currentProject.ScanDirectory();
                _currentProject.Save();
            }
            RefreshExplorer();
        }

        // ===== AI CHAT =====

        private async void BtnAISend_Click(object? sender, EventArgs e)
        {
            await SendAIMessage();
        }

        private async void TxtAIInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Shift)
            {
                // Shift+Enter: insert a newline at the caret position
                e.SuppressKeyPress = true;
                var selStart = txtAIInput.SelectionStart;
                txtAIInput.Text = txtAIInput.Text.Insert(selStart, Environment.NewLine);
                txtAIInput.SelectionStart = selStart + Environment.NewLine.Length;
            }
            else if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                await SendAIMessage();
            }
        }

        private async Task SendAIMessage()
        {
            var message = txtAIInput.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            txtAIInput.Clear();
            AppendAIChat($"You: {message}\n", Color.FromArgb(86, 156, 214));
            _chatHistory.Add("You", message);

            // Ensure the AI chat panel is visible
            if (splitCenter.Panel2Collapsed)
                splitCenter.Panel2Collapsed = false;

            btnAISend.Enabled = false;
            btnAISend.Text = "Thinking...";
            SetStatus("AI is thinking...");

            // Cancel any previous in-flight request
            _aiCts?.Cancel();
            _aiCts = new CancellationTokenSource();
            var ct = _aiCts.Token;

            try
            {
                // Build full project context if a project is open
                string? projectContext = null;
                string? activeFileName = null;
                if (_currentProject != null)
                {
                    // Save all files first so the AI sees the latest on-disk content
                    SaveAllFiles();
                    projectContext = KoboldCppService.BuildProjectContext(_currentProject);

                    if (tabEditor.SelectedTab is EditorTabPage activeTab)
                        activeFileName = _currentProject.GetRelativePath(activeTab.FilePath);
                }

                var response = await _aiService.GenerateAsync(message, projectContext, activeFileName, ct)
                    .ConfigureAwait(true);

                if (ct.IsCancellationRequested) return;

                // Guard against empty/null responses
                if (string.IsNullOrWhiteSpace(response))
                {
                    AppendAIChat("AI: [Empty response received]\n\n", Color.FromArgb(200, 100, 100));
                    SetStatus("AI returned empty response");
                    return;
                }

                // Apply any file edits embedded in the response
                var writtenFiles = new List<string>();
                if (_currentProject != null)
                {
                    writtenFiles = KoboldCppService.ApplyFileEdits(response, _currentProject);
                }

                // Show the chat-friendly version (FILE blocks replaced with "[file saved]")
                var displayText = KoboldCppService.StripFileBlocks(response);
                if (!string.IsNullOrWhiteSpace(displayText))
                {
                    AppendAIChat($"AI: {displayText}\n", Color.FromArgb(78, 201, 176));
                }

                // Report which files were written
                if (writtenFiles.Count > 0)
                {
                    var fileList = string.Join(", ", writtenFiles);
                    AppendAIChat($"[Saved {writtenFiles.Count} file(s): {fileList}]\n",
                        Color.FromArgb(220, 220, 170));

                    // Reload any open tabs whose files were modified by the AI
                    ReloadModifiedTabs(writtenFiles);

                    // Refresh the explorer in case new files were added
                    RefreshExplorer();
                }
                else if (string.IsNullOrWhiteSpace(displayText))
                {
                    // Neither text nor file edits — show the raw response for debugging
                    AppendAIChat($"AI: {response}\n", Color.FromArgb(78, 201, 176));
                }

                AppendAIChat("\n", Color.FromArgb(78, 201, 176));

                // Record AI response to chat history
                var historyText = writtenFiles.Count > 0
                    ? displayText + $"\n[Saved {writtenFiles.Count} file(s): {string.Join(", ", writtenFiles)}]"
                    : (string.IsNullOrWhiteSpace(displayText) ? response : displayText);
                _chatHistory.Add("AI", historyText);
                SaveChatHistory();

                SetStatus(writtenFiles.Count > 0
                    ? $"AI response received — {writtenFiles.Count} file(s) updated"
                    : "AI response received");
            }
            catch (OperationCanceledException)
            {
                AppendAIChat("[Request cancelled]\n\n", Color.FromArgb(200, 100, 100));
                SetStatus("AI request cancelled");
            }
            catch (Exception ex)
            {
                AppendAIChat($"[Error: {ex.Message}]\n\n", Color.FromArgb(200, 100, 100));
                SetStatus("AI request failed");
            }
            finally
            {
                btnAISend.Enabled = true;
                btnAISend.Text = "Send";
            }
        }

        /// <summary>
        /// Reloads the content of any open editor tabs whose files were
        /// modified by the AI, so the user sees the updated code immediately.
        /// </summary>
        private void ReloadModifiedTabs(List<string> relPaths)
        {
            if (_currentProject == null) return;

            foreach (var relPath in relPaths)
            {
                var absPath = _currentProject.GetAbsolutePath(relPath);
                foreach (TabPage tab in tabEditor.TabPages)
                {
                    if (tab is EditorTabPage etp &&
                        string.Equals(etp.FilePath, absPath, StringComparison.OrdinalIgnoreCase))
                    {
                        etp.LoadContent();
                        break;
                    }
                }
            }
        }

        private void AppendAIChat(string text, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(() => AppendAIChat(text, color));
                return;
            }
            txtAIChat.SelectionStart = txtAIChat.TextLength;
            txtAIChat.SelectionLength = 0;
            txtAIChat.SelectionColor = color;
            txtAIChat.AppendText(text);
            txtAIChat.ScrollToCaret();
        }

        // ===== HELPERS =====

        private void AppendOutput(string text)
        {
            if (InvokeRequired)
            {
                Invoke(() => AppendOutput(text));
                return;
            }
            txtOutput.AppendText(text + Environment.NewLine);
            txtOutput.ScrollToCaret();
        }

        private void SetStatus(string status)
        {
            if (InvokeRequired)
            {
                Invoke(() => SetStatus(status));
                return;
            }
            lblStatus.Text = status;
        }

        private void UpdateTitle()
        {
            Text = _currentProject != null
                ? $"KoPilot IDE - {_currentProject.Name}"
                : "KoPilot IDE";
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            // Build all menu items in code so the Designer cannot strip them
            SetupMenus();

            // Configure error list columns and owner-draw (done in code so the
            // Designer can't strip them on regeneration)
            SetupErrorList();

            // Set explorer width after layout so DPI scaling doesn't inflate the value
            splitMain.SplitterDistance = 220;

            // AI chat panel width (right side)
            splitCenter.SplitterDistance = splitCenter.Width - 350;

            LoadRecentProjects();
            RefreshRecentProjectsMenu();

            // Handle startup actions from OpenForm
            if (!string.IsNullOrEmpty(ProjectToLoadOnStart))
            {
                LoadProject(ProjectToLoadOnStart);
            }
            else if (RequestNewProjectOnLoad)
            {
                BeginInvoke(() => OnNewProject(null, EventArgs.Empty));
            }
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            SaveChatHistory();
            if (!ConfirmCloseCurrentProject())
                e.Cancel = true;
        }

        // ===== BUILD ERROR PARSING =====

        private void SetupErrorList()
        {
            lvErrors.Columns.Clear();
            lvErrors.Columns.AddRange([
                new ColumnHeader { Text = "Severity", Width = 70 },
                new ColumnHeader { Text = "Code",     Width = 80 },
                new ColumnHeader { Text = "Message",  Width = 400 },
                new ColumnHeader { Text = "File",     Width = 140 },
                new ColumnHeader { Text = "Line",     Width = 50 },
                new ColumnHeader { Text = "Col",      Width = 50 }
            ]);

            lvErrors.OwnerDraw = true;
            lvErrors.DrawColumnHeader += LvErrors_DrawColumnHeader;
            lvErrors.DrawItem += LvErrors_DrawItem;
            lvErrors.DrawSubItem += LvErrors_DrawSubItem;
        }

        // ===== AI CHAT HISTORY =====

        private void LoadChatHistory()
        {
            if (_currentProject == null) return;

            var historyPath = ChatHistory.GetHistoryPath(_currentProject);
            _chatHistory = ChatHistory.Load(historyPath);

            RestoreChatDisplay();
        }

        private void SaveChatHistory()
        {
            if (_currentProject == null) return;

            try
            {
                var historyPath = ChatHistory.GetHistoryPath(_currentProject);
                _chatHistory.Save(historyPath);
            }
            catch { }
        }

        private void RestoreChatDisplay()
        {
            txtAIChat.Clear();
            foreach (var msg in _chatHistory.Messages)
            {
                var color = msg.Role == "You"
                    ? Color.FromArgb(86, 156, 214)
                    : Color.FromArgb(78, 201, 176);
                AppendAIChat($"{msg.Role}: {msg.Text}\n\n", color);
            }
        }

        private void OnNewChat(object? sender, EventArgs e)
        {
            SaveChatHistory();
            _chatHistory.Clear();
            txtAIChat.Clear();
            SetStatus("New chat started");
        }

        private void OnExportChat(object? sender, EventArgs e)
        {
            if (_chatHistory.Messages.Count == 0)
            {
                MessageBox.Show("No chat messages to export.", "Export Chat",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Title = "Export Chat History",
                Filter = "Text Files (*.txt)|*.txt|Markdown (*.md)|*.md|All Files (*.*)|*.*",
                FileName = _currentProject != null
                    ? $"{_currentProject.Name}_chat_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                    : $"chat_{DateTime.Now:yyyyMMdd_HHmms}.txt",
                InitialDirectory = _currentProject?.RootPath
                    ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(sfd.FileName, _chatHistory.ExportToText());
                    SetStatus($"Chat exported to {Path.GetFileName(sfd.FileName)}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to export chat: {ex.Message}", "Export Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnImportChat(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Import Chat History",
                Filter = "All Supported Formats (*.txt;*.md;*.json;*.chat.json)|*.txt;*.md;*.json|Text Files (*.txt)|*.txt|Markdown (*.md)|*.md|JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                InitialDirectory = _currentProject?.RootPath
                    ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                var imported = ChatHistory.ImportFromFile(ofd.FileName);

                if (imported.Messages.Count == 0)
                {
                    MessageBox.Show("The selected file contains no chat messages.", "Import Chat",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Ask whether to replace or append to the current chat
                if (_chatHistory.Messages.Count > 0)
                {
                    var result = MessageBox.Show(
                        $"Import {imported.Messages.Count} message(s).\n\n" +
                        "'Yes' = Replace current chat\n" +
                        "'No' = Append to current chat",
                        "Import Chat",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel) return;

                    if (result == DialogResult.Yes)
                    {
                        _chatHistory.Clear();
                    }
                }

                foreach (var msg in imported.Messages)
                {
                    _chatHistory.Messages.Add(msg);
                }

                SaveChatHistory();
                RestoreChatDisplay();
                SetStatus($"Imported {imported.Messages.Count} message(s) from {Path.GetFileName(ofd.FileName)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to import chat: {ex.Message}", "Import Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== BUILD ERROR PARSING =====

        /// <summary>
        /// Parses MSBuild-style diagnostics from dotnet build output and populates
        /// the Errors list view. Matches lines like:
        ///   C:\path\File.cs(12,5): error CS1002: ; expected
        ///   C:\path\File.cs(12,5): warning CS0168: ...
        /// </summary>
        private void ParseBuildErrors(string buildOutput)
        {
            lvErrors.Items.Clear();

            // MSBuild diagnostic format: filepath(line,col): severity code: message
            var regex = new Regex(
                @"^(.+?)\((\d+),(\d+)\):\s+(error|warning)\s+(\w+):\s+(.+)$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach (Match match in regex.Matches(buildOutput))
            {
                var filePath = match.Groups[1].Value.Trim();
                var line     = match.Groups[2].Value;
                var col      = match.Groups[3].Value;
                var severity = match.Groups[4].Value;
                var code     = match.Groups[5].Value;
                var message  = match.Groups[6].Value.Trim();

                var item = new ListViewItem(severity.ToUpperInvariant() == "ERROR" ? "Error" : "Warning");
                item.SubItems.Add(code);
                item.SubItems.Add(message);
                item.SubItems.Add(Path.GetFileName(filePath));
                item.SubItems.Add(line);
                item.SubItems.Add(col);

                // Store the full path so we can navigate on double-click
                item.Tag = filePath;

                item.ForeColor = severity.ToUpperInvariant() == "ERROR"
                    ? Color.FromArgb(240, 100, 100)
                    : Color.FromArgb(220, 220, 120);

                lvErrors.Items.Add(item);
            }
        }

        private void LvErrors_DoubleClick(object? sender, EventArgs e)
        {
            if (lvErrors.SelectedItems.Count == 0) return;
            var item = lvErrors.SelectedItems[0];

            var filePath = item.Tag as string;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

            int.TryParse(item.SubItems[4].Text, out int line);

            NavigateToFileAndLine(filePath, line);
        }

        private void LvErrors_DrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
        {
            using var brush = new SolidBrush(Color.FromArgb(37, 37, 38));
            e.Graphics.FillRectangle(brush, e.Bounds);
            using var textBrush = new SolidBrush(Color.FromArgb(180, 180, 180));
            var textRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 4, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, e.Header!.Text, lvErrors.Font, textRect,
                Color.FromArgb(180, 180, 180), TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        private void LvErrors_DrawItem(object? sender, DrawListViewItemEventArgs e)
        {
            // Let DrawSubItem handle all painting per-column.
            // Only fill the full-row background here so gaps between
            // sub-items (if any) are not left unpainted.
            var bgColor = e.Item.Selected
                ? Color.FromArgb(62, 62, 64)
                : Color.FromArgb(30, 30, 30);
            using var brush = new SolidBrush(bgColor);
            e.Graphics.FillRectangle(brush, e.Bounds);
        }

        private void LvErrors_DrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
        {
            // For column 0 with FullRowSelect, e.Bounds spans the entire row.
            // Compute the real column bounds so we don't wipe other columns.
            var bounds = e.Bounds;
            if (e.ColumnIndex == 0)
            {
                bounds = new Rectangle(
                    e.Bounds.X,
                    e.Bounds.Y,
                    lvErrors.Columns[0].Width,
                    e.Bounds.Height);
            }

            var bgColor = e.Item!.Selected
                ? Color.FromArgb(62, 62, 64)
                : Color.FromArgb(30, 30, 30);
            using var bgBrush = new SolidBrush(bgColor);
            e.Graphics!.FillRectangle(bgBrush, bounds);

            var textRect = new Rectangle(bounds.X + 4, bounds.Y, bounds.Width - 4, bounds.Height);
            TextRenderer.DrawText(e.Graphics, e.SubItem!.Text, lvErrors.Font, textRect,
                e.Item.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        private void NavigateToFileAndLine(string absolutePath, int line)
        {
            OpenFileInEditor(absolutePath);

            if (tabEditor.SelectedTab is EditorTabPage etp && line >= 1)
            {
                var lineIndex = Math.Min(line - 1, etp.Editor.Lines.Length - 1);
                if (lineIndex >= 0)
                {
                    var charIndex = etp.Editor.GetFirstCharIndexFromLine(lineIndex);
                    if (charIndex >= 0)
                    {
                        etp.Editor.SelectionStart = charIndex;
                        etp.Editor.SelectionLength = 0;
                        etp.Editor.ScrollToCaret();
                        etp.Editor.Focus();
                    }
                }
            }
        }

        // ===== RECENT PROJECTS =====

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

        private void AddToRecentProjects(string projectFilePath)
        {
            // Remove duplicates (case-insensitive) then insert at front
            _recentProjects.RemoveAll(p =>
                string.Equals(p, projectFilePath, StringComparison.OrdinalIgnoreCase));
            _recentProjects.Insert(0, projectFilePath);

            // Trim to max
            while (_recentProjects.Count > MaxRecentProjects)
                _recentProjects.RemoveAt(_recentProjects.Count - 1);

            SaveRecentProjects();
            RefreshRecentProjectsMenu();
        }

        private void RefreshRecentProjectsMenu()
        {
            mnuRecentProjects.DropDownItems.Clear();

            if (_recentProjects.Count == 0)
            {
                var empty = new ToolStripMenuItem("(none)") { Enabled = false };
                mnuRecentProjects.DropDownItems.Add(empty);
                return;
            }

            foreach (var path in _recentProjects)
            {
                var displayName = Path.GetFileNameWithoutExtension(path);
                var menuItem = new ToolStripMenuItem($"{displayName}   ({path})");
                menuItem.Tag = path;
                menuItem.Click += OnRecentProjectClick;
                mnuRecentProjects.DropDownItems.Add(menuItem);
            }

            mnuRecentProjects.DropDownItems.Add(new ToolStripSeparator());
            var clearItem = new ToolStripMenuItem("Clear Recent Projects");
            clearItem.Click += (_, _) =>
            {
                _recentProjects.Clear();
                SaveRecentProjects();
                RefreshRecentProjectsMenu();
            };
            mnuRecentProjects.DropDownItems.Add(clearItem);
        }

        private void OnRecentProjectClick(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem mi || mi.Tag is not string path) return;

            if (!File.Exists(path))
            {
                MessageBox.Show($"Project file not found:\n{path}", "Recent Projects",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _recentProjects.Remove(path);
                SaveRecentProjects();
                RefreshRecentProjectsMenu();
                return;
            }

            if (!ConfirmCloseCurrentProject()) return;
            LoadProject(path);
        }
    }
}
