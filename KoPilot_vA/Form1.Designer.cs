namespace KoPilot_vA
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip = new MenuStrip();
            mnuFile = new ToolStripMenuItem();
            mnuSep1 = new ToolStripSeparator();
            mnuSep2 = new ToolStripSeparator();
            mnuRecentProjects = new ToolStripMenuItem();
            mnuSep3 = new ToolStripSeparator();
            mnuEdit = new ToolStripMenuItem();
            mnuSep4 = new ToolStripSeparator();
            mnuSep5 = new ToolStripSeparator();
            mnuProject = new ToolStripMenuItem();
            mnuSep6 = new ToolStripSeparator();
            mnuBuild = new ToolStripMenuItem();
            mnuTools = new ToolStripMenuItem();
            mnuWindow = new ToolStripMenuItem();
            mnuHelp = new ToolStripMenuItem();
            toolStrip = new ToolStrip();
            tsbNewProject = new ToolStripButton();
            tsbOpenProject = new ToolStripButton();
            tsbSave = new ToolStripButton();
            tsbSaveAll = new ToolStripButton();
            tsbSep1 = new ToolStripSeparator();
            tsbBuild = new ToolStripButton();
            tsbRun = new ToolStripButton();
            tsbSep2 = new ToolStripSeparator();
            tsbUndo = new ToolStripButton();
            tsbRedo = new ToolStripButton();
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            lblProjectName = new ToolStripStatusLabel();
            lblLineCol = new ToolStripStatusLabel();
            splitMain = new SplitContainer();
            panelExplorer = new Panel();
            treeFiles = new TreeView();
            treeContextMenu = new ContextMenuStrip(components);
            lblExplorer = new Label();
            splitCenter = new SplitContainer();
            splitRight = new SplitContainer();
            tabEditor = new TabControl();
            tabBottom = new TabControl();
            tabOutput = new TabPage();
            txtOutput = new RichTextBox();
            tabErrors = new TabPage();
            lvErrors = new DoubleBufferedListView();
            panelAIChat = new Panel();
            txtAIChat = new RichTextBox();
            panelAIBottom = new Panel();
            panelAIInput = new Panel();
            txtAIInput = new TextBox();
            btnAISend = new Button();
            lblAIInputHint = new Label();
            panelAIInputSep = new Panel();
            panelAIFooter = new Panel();
            panelAIHeader = new Panel();
            lblAITitle = new Label();
            btnAINewChat = new Button();
            btnAIImport = new Button();
            btnAIExport = new Button();
            btnAIChatClose = new Button();
            menuStrip.SuspendLayout();
            toolStrip.SuspendLayout();
            statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            panelExplorer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitCenter).BeginInit();
            splitCenter.Panel1.SuspendLayout();
            splitCenter.Panel2.SuspendLayout();
            splitCenter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitRight).BeginInit();
            splitRight.Panel1.SuspendLayout();
            splitRight.Panel2.SuspendLayout();
            splitRight.SuspendLayout();
            tabBottom.SuspendLayout();
            tabOutput.SuspendLayout();
            tabErrors.SuspendLayout();
            panelAIChat.SuspendLayout();
            panelAIBottom.SuspendLayout();
            panelAIInput.SuspendLayout();
            panelAIHeader.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.BackColor = Color.FromArgb(45, 45, 48);
            menuStrip.ForeColor = Color.FromArgb(220, 220, 220);
            menuStrip.ImageScalingSize = new Size(20, 20);
            menuStrip.Items.AddRange(new ToolStripItem[] { mnuFile, mnuEdit, mnuProject, mnuBuild, mnuTools, mnuWindow, mnuHelp });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1280, 28);
            menuStrip.TabIndex = 3;
            // 
            // mnuFile
            // 
            mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuSep1, mnuSep2, mnuRecentProjects, mnuSep3 });
            mnuFile.Name = "mnuFile";
            mnuFile.Size = new Size(46, 24);
            mnuFile.Text = "&File";
            // 
            // mnuSep1
            // 
            mnuSep1.Name = "mnuSep1";
            mnuSep1.Size = new Size(190, 6);
            // 
            // mnuSep2
            // 
            mnuSep2.Name = "mnuSep2";
            mnuSep2.Size = new Size(190, 6);
            // 
            // mnuRecentProjects
            // 
            mnuRecentProjects.Name = "mnuRecentProjects";
            mnuRecentProjects.Size = new Size(193, 26);
            mnuRecentProjects.Text = "Recent Projects";
            // 
            // mnuSep3
            // 
            mnuSep3.Name = "mnuSep3";
            mnuSep3.Size = new Size(190, 6);
            // 
            // mnuEdit
            // 
            mnuEdit.DropDownItems.AddRange(new ToolStripItem[] { mnuSep4, mnuSep5 });
            mnuEdit.Name = "mnuEdit";
            mnuEdit.Size = new Size(49, 24);
            mnuEdit.Text = "&Edit";
            // 
            // mnuSep4
            // 
            mnuSep4.Name = "mnuSep4";
            mnuSep4.Size = new Size(71, 6);
            // 
            // mnuSep5
            // 
            mnuSep5.Name = "mnuSep5";
            mnuSep5.Size = new Size(71, 6);
            // 
            // mnuProject
            // 
            mnuProject.DropDownItems.AddRange(new ToolStripItem[] { mnuSep6 });
            mnuProject.Name = "mnuProject";
            mnuProject.Size = new Size(69, 24);
            mnuProject.Text = "&Project";
            // 
            // mnuSep6
            // 
            mnuSep6.Name = "mnuSep6";
            mnuSep6.Size = new Size(71, 6);
            // 
            // mnuBuild
            // 
            mnuBuild.Name = "mnuBuild";
            mnuBuild.Size = new Size(57, 24);
            mnuBuild.Text = "&Build";
            // 
            // mnuTools
            // 
            mnuTools.Name = "mnuTools";
            mnuTools.Size = new Size(58, 24);
            mnuTools.Text = "&Tools";
            // 
            // mnuWindow
            // 
            mnuWindow.Name = "mnuWindow";
            mnuWindow.Size = new Size(78, 24);
            mnuWindow.Text = "&Window";
            // 
            // mnuHelp
            // 
            mnuHelp.Name = "mnuHelp";
            mnuHelp.Size = new Size(55, 24);
            mnuHelp.Text = "&Help";
            // 
            // toolStrip
            // 
            toolStrip.BackColor = Color.FromArgb(45, 45, 48);
            toolStrip.ForeColor = Color.FromArgb(220, 220, 220);
            toolStrip.ImageScalingSize = new Size(20, 20);
            toolStrip.Items.AddRange(new ToolStripItem[] { tsbNewProject, tsbOpenProject, tsbSave, tsbSaveAll, tsbSep1, tsbBuild, tsbRun, tsbSep2, tsbUndo, tsbRedo });
            toolStrip.Location = new Point(0, 28);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new Size(1280, 27);
            toolStrip.TabIndex = 2;
            // 
            // tsbNewProject
            // 
            tsbNewProject.Name = "tsbNewProject";
            tsbNewProject.Size = new Size(93, 24);
            tsbNewProject.Text = "New Project";
            tsbNewProject.Click += OnNewProject;
            // 
            // tsbOpenProject
            // 
            tsbOpenProject.Name = "tsbOpenProject";
            tsbOpenProject.Size = new Size(49, 24);
            tsbOpenProject.Text = "Open";
            tsbOpenProject.Click += OnOpenProject;
            // 
            // tsbSave
            // 
            tsbSave.Name = "tsbSave";
            tsbSave.Size = new Size(44, 24);
            tsbSave.Text = "Save";
            tsbSave.Click += OnSaveFile;
            // 
            // tsbSaveAll
            // 
            tsbSaveAll.Name = "tsbSaveAll";
            tsbSaveAll.Size = new Size(66, 24);
            tsbSaveAll.Text = "Save All";
            tsbSaveAll.Click += OnSaveAll;
            // 
            // tsbSep1
            // 
            tsbSep1.Name = "tsbSep1";
            tsbSep1.Size = new Size(6, 27);
            // 
            // tsbBuild
            // 
            tsbBuild.Name = "tsbBuild";
            tsbBuild.Size = new Size(47, 24);
            tsbBuild.Text = "Build";
            tsbBuild.Click += OnBuildProject;
            // 
            // tsbRun
            // 
            tsbRun.Name = "tsbRun";
            tsbRun.Size = new Size(49, 24);
            tsbRun.Text = "? Run";
            tsbRun.Click += OnRunProject;
            // 
            // tsbSep2
            // 
            tsbSep2.Name = "tsbSep2";
            tsbSep2.Size = new Size(6, 27);
            // 
            // tsbUndo
            // 
            tsbUndo.Name = "tsbUndo";
            tsbUndo.Size = new Size(49, 24);
            tsbUndo.Text = "Undo";
            tsbUndo.Click += OnUndo;
            // 
            // tsbRedo
            // 
            tsbRedo.Name = "tsbRedo";
            tsbRedo.Size = new Size(48, 24);
            tsbRedo.Text = "Redo";
            tsbRedo.Click += OnRedo;
            // 
            // statusStrip
            // 
            statusStrip.BackColor = Color.FromArgb(0, 122, 204);
            statusStrip.ImageScalingSize = new Size(20, 20);
            statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus, lblProjectName, lblLineCol });
            statusStrip.Location = new Point(0, 770);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1280, 30);
            statusStrip.TabIndex = 4;
            // 
            // lblStatus
            // 
            lblStatus.ForeColor = Color.White;
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(50, 24);
            lblStatus.Text = "Ready";
            // 
            // lblProjectName
            // 
            lblProjectName.BorderSides = ToolStripStatusLabelBorderSides.Left;
            lblProjectName.ForeColor = Color.White;
            lblProjectName.Name = "lblProjectName";
            lblProjectName.Size = new Size(83, 24);
            lblProjectName.Text = "No Project";
            // 
            // lblLineCol
            // 
            lblLineCol.Alignment = ToolStripItemAlignment.Right;
            lblLineCol.ForeColor = Color.White;
            lblLineCol.Name = "lblLineCol";
            lblLineCol.Size = new Size(77, 24);
            lblLineCol.Text = "Ln 1, Col 1";
            // 
            // splitMain
            // 
            splitMain.BackColor = Color.FromArgb(37, 37, 38);
            splitMain.Dock = DockStyle.Fill;
            splitMain.Location = new Point(0, 55);
            splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            splitMain.Panel1.Controls.Add(panelExplorer);
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(splitCenter);
            splitMain.Size = new Size(1280, 715);
            splitMain.SplitterDistance = 426;
            splitMain.TabIndex = 1;
            // 
            // panelExplorer
            // 
            panelExplorer.BackColor = Color.FromArgb(37, 37, 38);
            panelExplorer.Controls.Add(treeFiles);
            panelExplorer.Controls.Add(lblExplorer);
            panelExplorer.Dock = DockStyle.Fill;
            panelExplorer.Location = new Point(0, 0);
            panelExplorer.Name = "panelExplorer";
            panelExplorer.Size = new Size(426, 715);
            panelExplorer.TabIndex = 0;
            // 
            // treeFiles
            // 
            treeFiles.BackColor = Color.FromArgb(37, 37, 38);
            treeFiles.BorderStyle = BorderStyle.None;
            treeFiles.ContextMenuStrip = treeContextMenu;
            treeFiles.Dock = DockStyle.Fill;
            treeFiles.ForeColor = Color.FromArgb(220, 220, 220);
            treeFiles.HideSelection = false;
            treeFiles.Location = new Point(0, 0);
            treeFiles.Name = "treeFiles";
            treeFiles.ShowLines = false;
            treeFiles.Size = new Size(426, 715);
            treeFiles.TabIndex = 0;
            treeFiles.NodeMouseDoubleClick += TreeFiles_NodeMouseDoubleClick;
            // 
            // treeContextMenu
            // 
            treeContextMenu.BackColor = Color.FromArgb(45, 45, 48);
            treeContextMenu.ForeColor = Color.White;
            treeContextMenu.ImageScalingSize = new Size(20, 20);
            treeContextMenu.Name = "treeContextMenu";
            treeContextMenu.Size = new Size(61, 4);
            // 
            // lblExplorer
            // 
            lblExplorer.Location = new Point(0, 0);
            lblExplorer.Name = "lblExplorer";
            lblExplorer.Size = new Size(100, 23);
            lblExplorer.TabIndex = 1;
            // 
            // splitCenter
            // 
            splitCenter.BackColor = Color.FromArgb(30, 30, 30);
            splitCenter.Dock = DockStyle.Fill;
            splitCenter.FixedPanel = FixedPanel.Panel2;
            splitCenter.Location = new Point(0, 0);
            splitCenter.Name = "splitCenter";
            // 
            // splitCenter.Panel1
            // 
            splitCenter.Panel1.Controls.Add(splitRight);
            // 
            // splitCenter.Panel2
            // 
            splitCenter.Panel2.Controls.Add(panelAIChat);
            splitCenter.Size = new Size(850, 715);
            splitCenter.SplitterDistance = 779;
            splitCenter.TabIndex = 0;
            // 
            // splitRight
            // 
            splitRight.BackColor = Color.FromArgb(30, 30, 30);
            splitRight.Dock = DockStyle.Fill;
            splitRight.Location = new Point(0, 0);
            splitRight.Name = "splitRight";
            splitRight.Orientation = Orientation.Horizontal;
            // 
            // splitRight.Panel1
            // 
            splitRight.Panel1.Controls.Add(tabEditor);
            // 
            // splitRight.Panel2
            // 
            splitRight.Panel2.Controls.Add(tabBottom);
            splitRight.Size = new Size(779, 715);
            splitRight.SplitterDistance = 506;
            splitRight.TabIndex = 0;
            // 
            // tabEditor
            // 
            tabEditor.Dock = DockStyle.Fill;
            tabEditor.Location = new Point(0, 0);
            tabEditor.Name = "tabEditor";
            tabEditor.SelectedIndex = 0;
            tabEditor.Size = new Size(779, 506);
            tabEditor.TabIndex = 0;
            tabEditor.SelectedIndexChanged += TabEditor_SelectedIndexChanged;
            tabEditor.MouseClick += TabEditor_MouseClick;
            tabEditor.MouseUp += TabEditor_MouseUp;
            // 
            // tabBottom
            // 
            tabBottom.Controls.Add(tabOutput);
            tabBottom.Controls.Add(tabErrors);
            tabBottom.Dock = DockStyle.Fill;
            tabBottom.Location = new Point(0, 0);
            tabBottom.Name = "tabBottom";
            tabBottom.SelectedIndex = 0;
            tabBottom.Size = new Size(779, 205);
            tabBottom.TabIndex = 0;
            // 
            // tabOutput
            // 
            tabOutput.BackColor = Color.FromArgb(30, 30, 30);
            tabOutput.Controls.Add(txtOutput);
            tabOutput.Location = new Point(4, 29);
            tabOutput.Name = "tabOutput";
            tabOutput.Size = new Size(771, 172);
            tabOutput.TabIndex = 0;
            tabOutput.Text = "Output";
            // 
            // txtOutput
            // 
            txtOutput.BackColor = Color.FromArgb(30, 30, 30);
            txtOutput.BorderStyle = BorderStyle.None;
            txtOutput.Dock = DockStyle.Fill;
            txtOutput.Font = new Font("Consolas", 10F);
            txtOutput.ForeColor = Color.FromArgb(220, 220, 220);
            txtOutput.Location = new Point(0, 0);
            txtOutput.Name = "txtOutput";
            txtOutput.ReadOnly = true;
            txtOutput.Size = new Size(771, 172);
            txtOutput.TabIndex = 0;
            txtOutput.Text = "";
            txtOutput.WordWrap = false;
            // 
            // tabErrors
            // 
            tabErrors.BackColor = Color.FromArgb(30, 30, 30);
            tabErrors.Controls.Add(lvErrors);
            tabErrors.Location = new Point(4, 29);
            tabErrors.Name = "tabErrors";
            tabErrors.Size = new Size(771, 172);
            tabErrors.TabIndex = 1;
            tabErrors.Text = "Errors";
            // 
            // lvErrors
            // 
            lvErrors.BackColor = Color.FromArgb(30, 30, 30);
            lvErrors.BorderStyle = BorderStyle.None;
            lvErrors.Dock = DockStyle.Fill;
            lvErrors.Font = new Font("Consolas", 10F);
            lvErrors.ForeColor = Color.FromArgb(220, 220, 220);
            lvErrors.FullRowSelect = true;
            lvErrors.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvErrors.Location = new Point(0, 0);
            lvErrors.Name = "lvErrors";
            lvErrors.Size = new Size(771, 172);
            lvErrors.TabIndex = 0;
            lvErrors.UseCompatibleStateImageBehavior = false;
            lvErrors.View = View.Details;
            lvErrors.DoubleClick += LvErrors_DoubleClick;
            // 
            // panelAIChat
            // 
            panelAIChat.BackColor = Color.FromArgb(30, 30, 30);
            panelAIChat.Controls.Add(txtAIChat);
            panelAIChat.Controls.Add(panelAIBottom);
            panelAIChat.Controls.Add(panelAIHeader);
            panelAIChat.Dock = DockStyle.Fill;
            panelAIChat.Location = new Point(0, 0);
            panelAIChat.MinimumSize = new Size(280, 0);
            panelAIChat.Name = "panelAIChat";
            panelAIChat.Size = new Size(280, 715);
            panelAIChat.TabIndex = 0;
            // 
            // txtAIChat
            // 
            txtAIChat.BackColor = Color.FromArgb(30, 30, 30);
            txtAIChat.BorderStyle = BorderStyle.None;
            txtAIChat.Dock = DockStyle.Fill;
            txtAIChat.Font = new Font("Consolas", 10F);
            txtAIChat.ForeColor = Color.FromArgb(220, 220, 220);
            txtAIChat.Location = new Point(0, 32);
            txtAIChat.Name = "txtAIChat";
            txtAIChat.ReadOnly = true;
            txtAIChat.Size = new Size(280, 553);
            txtAIChat.TabIndex = 0;
            txtAIChat.Text = "";
            // 
            // panelAIBottom
            // 
            panelAIBottom.BackColor = Color.FromArgb(37, 37, 38);
            panelAIBottom.Controls.Add(panelAIInput);
            panelAIBottom.Controls.Add(lblAIInputHint);
            panelAIBottom.Controls.Add(panelAIInputSep);
            panelAIBottom.Controls.Add(panelAIFooter);
            panelAIBottom.Dock = DockStyle.Bottom;
            panelAIBottom.Location = new Point(0, 585);
            panelAIBottom.Name = "panelAIBottom";
            panelAIBottom.Size = new Size(280, 130);
            panelAIBottom.TabIndex = 1;
            // 
            // panelAIInput
            // 
            panelAIInput.BackColor = Color.FromArgb(45, 45, 48);
            panelAIInput.Controls.Add(txtAIInput);
            panelAIInput.Controls.Add(btnAISend);
            panelAIInput.Dock = DockStyle.Fill;
            panelAIInput.Location = new Point(0, 19);
            panelAIInput.Name = "panelAIInput";
            panelAIInput.Padding = new Padding(4);
            panelAIInput.Size = new Size(280, 83);
            panelAIInput.TabIndex = 0;
            // 
            // txtAIInput
            // 
            txtAIInput.BackColor = Color.FromArgb(30, 30, 30);
            txtAIInput.Dock = DockStyle.Fill;
            txtAIInput.Font = new Font("Consolas", 10F);
            txtAIInput.ForeColor = Color.White;
            txtAIInput.Location = new Point(4, 4);
            txtAIInput.Multiline = true;
            txtAIInput.Name = "txtAIInput";
            txtAIInput.ScrollBars = ScrollBars.Vertical;
            txtAIInput.Size = new Size(202, 75);
            txtAIInput.TabIndex = 0;
            txtAIInput.KeyDown += TxtAIInput_KeyDown;
            // 
            // btnAISend
            // 
            btnAISend.BackColor = Color.FromArgb(0, 122, 204);
            btnAISend.Dock = DockStyle.Right;
            btnAISend.FlatStyle = FlatStyle.Flat;
            btnAISend.ForeColor = Color.White;
            btnAISend.Location = new Point(206, 4);
            btnAISend.Name = "btnAISend";
            btnAISend.Size = new Size(70, 75);
            btnAISend.TabIndex = 1;
            btnAISend.Text = "Send";
            btnAISend.UseVisualStyleBackColor = false;
            btnAISend.Click += BtnAISend_Click;
            // 
            // lblAIInputHint
            // 
            lblAIInputHint.BackColor = Color.FromArgb(37, 37, 38);
            lblAIInputHint.Dock = DockStyle.Top;
            lblAIInputHint.Font = new Font("Segoe UI", 7.5F);
            lblAIInputHint.ForeColor = Color.FromArgb(100, 100, 100);
            lblAIInputHint.Location = new Point(0, 1);
            lblAIInputHint.Name = "lblAIInputHint";
            lblAIInputHint.Size = new Size(280, 18);
            lblAIInputHint.TabIndex = 1;
            lblAIInputHint.Text = "  Shift+Enter for new line";
            lblAIInputHint.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelAIInputSep
            // 
            panelAIInputSep.BackColor = Color.FromArgb(62, 62, 66);
            panelAIInputSep.Dock = DockStyle.Top;
            panelAIInputSep.Location = new Point(0, 0);
            panelAIInputSep.Name = "panelAIInputSep";
            panelAIInputSep.Size = new Size(280, 1);
            panelAIInputSep.TabIndex = 2;
            // 
            // panelAIFooter
            // 
            panelAIFooter.BackColor = Color.FromArgb(37, 37, 38);
            panelAIFooter.Dock = DockStyle.Bottom;
            panelAIFooter.Location = new Point(0, 102);
            panelAIFooter.Name = "panelAIFooter";
            panelAIFooter.Size = new Size(280, 28);
            panelAIFooter.TabIndex = 3;
            // 
            // panelAIHeader
            // 
            panelAIHeader.BackColor = Color.FromArgb(37, 37, 38);
            panelAIHeader.Controls.Add(lblAITitle);
            panelAIHeader.Controls.Add(btnAINewChat);
            panelAIHeader.Controls.Add(btnAIImport);
            panelAIHeader.Controls.Add(btnAIExport);
            panelAIHeader.Controls.Add(btnAIChatClose);
            panelAIHeader.Dock = DockStyle.Top;
            panelAIHeader.Location = new Point(0, 0);
            panelAIHeader.Name = "panelAIHeader";
            panelAIHeader.Size = new Size(280, 32);
            panelAIHeader.TabIndex = 2;
            // 
            // lblAITitle
            // 
            lblAITitle.Location = new Point(0, 0);
            lblAITitle.Name = "lblAITitle";
            lblAITitle.Size = new Size(100, 23);
            lblAITitle.TabIndex = 0;
            // 
            // btnAINewChat
            // 
            btnAINewChat.BackColor = Color.FromArgb(37, 37, 38);
            btnAINewChat.Dock = DockStyle.Right;
            btnAINewChat.FlatAppearance.BorderSize = 0;
            btnAINewChat.FlatStyle = FlatStyle.Flat;
            btnAINewChat.Font = new Font("Segoe UI", 8.5F);
            btnAINewChat.ForeColor = Color.FromArgb(170, 170, 170);
            btnAINewChat.Location = new Point(54, 0);
            btnAINewChat.Name = "btnAINewChat";
            btnAINewChat.Size = new Size(74, 32);
            btnAINewChat.TabIndex = 1;
            btnAINewChat.Text = "New Chat";
            btnAINewChat.UseVisualStyleBackColor = false;
            btnAINewChat.Click += OnNewChat;
            // 
            // btnAIImport
            // 
            btnAIImport.BackColor = Color.FromArgb(37, 37, 38);
            btnAIImport.Dock = DockStyle.Right;
            btnAIImport.FlatAppearance.BorderSize = 0;
            btnAIImport.FlatStyle = FlatStyle.Flat;
            btnAIImport.Font = new Font("Segoe UI", 8.5F);
            btnAIImport.ForeColor = Color.FromArgb(170, 170, 170);
            btnAIImport.Location = new Point(128, 0);
            btnAIImport.Name = "btnAIImport";
            btnAIImport.Size = new Size(60, 32);
            btnAIImport.TabIndex = 2;
            btnAIImport.Text = "Import";
            btnAIImport.UseVisualStyleBackColor = false;
            btnAIImport.Click += OnImportChat;
            // 
            // btnAIExport
            // 
            btnAIExport.BackColor = Color.FromArgb(37, 37, 38);
            btnAIExport.Dock = DockStyle.Right;
            btnAIExport.FlatAppearance.BorderSize = 0;
            btnAIExport.FlatStyle = FlatStyle.Flat;
            btnAIExport.Font = new Font("Segoe UI", 8.5F);
            btnAIExport.ForeColor = Color.FromArgb(170, 170, 170);
            btnAIExport.Location = new Point(188, 0);
            btnAIExport.Name = "btnAIExport";
            btnAIExport.Size = new Size(60, 32);
            btnAIExport.TabIndex = 3;
            btnAIExport.Text = "Export";
            btnAIExport.UseVisualStyleBackColor = false;
            btnAIExport.Click += OnExportChat;
            // 
            // btnAIChatClose
            // 
            btnAIChatClose.BackColor = Color.FromArgb(37, 37, 38);
            btnAIChatClose.Dock = DockStyle.Right;
            btnAIChatClose.FlatAppearance.BorderSize = 0;
            btnAIChatClose.FlatStyle = FlatStyle.Flat;
            btnAIChatClose.Font = new Font("Segoe UI", 9F);
            btnAIChatClose.ForeColor = Color.FromArgb(170, 170, 170);
            btnAIChatClose.Location = new Point(248, 0);
            btnAIChatClose.Name = "btnAIChatClose";
            btnAIChatClose.Size = new Size(32, 32);
            btnAIChatClose.TabIndex = 4;
            btnAIChatClose.Text = "✕";
            btnAIChatClose.UseVisualStyleBackColor = false;
            btnAIChatClose.Click += OnToggleAIPanel;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(1280, 800);
            Controls.Add(splitMain);
            Controls.Add(toolStrip);
            Controls.Add(menuStrip);
            Controls.Add(statusStrip);
            if (Program.AppIcon != null) Icon = Program.AppIcon;
            MainMenuStrip = menuStrip;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "KoPilot IDE";
            WindowState = FormWindowState.Maximized;
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            panelExplorer.ResumeLayout(false);
            splitCenter.Panel1.ResumeLayout(false);
            splitCenter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitCenter).EndInit();
            splitCenter.ResumeLayout(false);
            splitRight.Panel1.ResumeLayout(false);
            splitRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitRight).EndInit();
            splitRight.ResumeLayout(false);
            tabBottom.ResumeLayout(false);
            tabOutput.ResumeLayout(false);
            tabErrors.ResumeLayout(false);
            panelAIChat.ResumeLayout(false);
            panelAIBottom.ResumeLayout(false);
            panelAIInput.ResumeLayout(false);
            panelAIInput.PerformLayout();
            panelAIHeader.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        // Menu
        private MenuStrip menuStrip;
        private ToolStripMenuItem mnuFile;
        private ToolStripMenuItem mnuNewProject;
        private ToolStripMenuItem mnuOpenProject;
        private ToolStripMenuItem mnuSaveProject;
        private ToolStripMenuItem mnuCloseProject;
        private ToolStripSeparator mnuSep1;
        private ToolStripMenuItem mnuNewFile;
        private ToolStripMenuItem mnuOpenFile;
        private ToolStripMenuItem mnuSaveFile;
        private ToolStripMenuItem mnuSaveAll;
        private ToolStripMenuItem mnuCloseFile;
        private ToolStripSeparator mnuSep2;
        private ToolStripMenuItem mnuRecentProjects;
        private ToolStripSeparator mnuSep3;
        private ToolStripMenuItem mnuExit;

        private ToolStripMenuItem mnuEdit;
        private ToolStripMenuItem mnuUndo;
        private ToolStripMenuItem mnuRedo;
        private ToolStripSeparator mnuSep4;
        private ToolStripMenuItem mnuCut;
        private ToolStripMenuItem mnuCopy;
        private ToolStripMenuItem mnuPaste;
        private ToolStripMenuItem mnuSelectAll;
        private ToolStripSeparator mnuSep5;
        private ToolStripMenuItem mnuFind;
        private ToolStripMenuItem mnuReplace;
        private ToolStripMenuItem mnuGoToLine;

        private ToolStripMenuItem mnuProject;
        private ToolStripMenuItem mnuAddFile;
        private ToolStripMenuItem mnuAddExistingFile;
        private ToolStripMenuItem mnuRemoveFile;
        private ToolStripMenuItem mnuRenameFile;
        private ToolStripMenuItem mnuDeleteFile;
        private ToolStripSeparator mnuSep6;
        private ToolStripMenuItem mnuProjectProperties;

        private ToolStripMenuItem mnuBuild;
        private ToolStripMenuItem mnuBuildProject;
        private ToolStripMenuItem mnuRun;
        private ToolStripMenuItem mnuRunWithoutDebug;
        private ToolStripMenuItem mnuClean;

        private ToolStripMenuItem mnuTools;
        private ToolStripMenuItem mnuSettings;
        private ToolStripMenuItem mnuAISettings;

        private ToolStripMenuItem mnuWindow;
        private ToolStripMenuItem mnuToggleExplorer;
        private ToolStripMenuItem mnuToggleOutput;
        private ToolStripMenuItem mnuToggleAIPanel;

        private ToolStripMenuItem mnuHelp;
        private ToolStripMenuItem mnuAbout;

        // Toolbar
        private ToolStrip toolStrip;
        private ToolStripButton tsbNewProject;
        private ToolStripButton tsbOpenProject;
        private ToolStripButton tsbSave;
        private ToolStripButton tsbSaveAll;
        private ToolStripSeparator tsbSep1;
        private ToolStripButton tsbBuild;
        private ToolStripButton tsbRun;
        private ToolStripSeparator tsbSep2;
        private ToolStripButton tsbUndo;
        private ToolStripButton tsbRedo;

        // Status
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;
        private ToolStripStatusLabel lblLineCol;
        private ToolStripStatusLabel lblProjectName;

        // Layout
        private SplitContainer splitMain;
        private SplitContainer splitCenter;
        private SplitContainer splitRight;

        // Explorer
        private Panel panelExplorer;
        private TreeView treeFiles;
        private ContextMenuStrip treeContextMenu;

        // Editor
        private TabControl tabEditor;

        // Bottom panels
        private TabControl tabBottom;
        private TabPage tabOutput;
        private RichTextBox txtOutput;
        private TabPage tabErrors;
        private DoubleBufferedListView lvErrors;

        // AI Chat panel (right side)
        private Panel panelAIChat;
        private Panel panelAIHeader;
        private Button btnAINewChat;
        private Button btnAIExport;
        private Button btnAIImport;
        private Button btnAIChatClose;
        private RichTextBox txtAIChat;
        private Panel panelAIBottom;
        private Panel panelAIInput;
        private Panel panelAIFooter;
        private Label lblAIInputHint;
        private TextBox txtAIInput;
        private Button btnAISend;
        private Label lblExplorer;
        private Panel panelAIInputSep;
        private Label lblAITitle;
    }

    internal class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        public DarkMenuRenderer() : base(new DarkColorTable()) { }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = Color.FromArgb(220, 220, 220);
            base.OnRenderItemText(e);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                using var brush = new SolidBrush(Color.FromArgb(62, 62, 64));
                e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
            }
            else
            {
                base.OnRenderMenuItemBackground(e);
            }
        }
    }

    internal class DarkColorTable : ProfessionalColorTable
    {
        public override Color MenuStripGradientBegin => Color.FromArgb(45, 45, 48);
        public override Color MenuStripGradientEnd => Color.FromArgb(45, 45, 48);
        public override Color MenuItemSelected => Color.FromArgb(62, 62, 64);
        public override Color MenuBorder => Color.FromArgb(51, 51, 55);
        public override Color MenuItemBorder => Color.FromArgb(62, 62, 64);
        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(62, 62, 64);
        public override Color MenuItemSelectedGradientEnd => Color.FromArgb(62, 62, 64);
        public override Color MenuItemPressedGradientBegin => Color.FromArgb(27, 27, 28);
        public override Color MenuItemPressedGradientEnd => Color.FromArgb(27, 27, 28);
        public override Color ImageMarginGradientBegin => Color.FromArgb(45, 45, 48);
        public override Color ImageMarginGradientMiddle => Color.FromArgb(45, 45, 48);
        public override Color ImageMarginGradientEnd => Color.FromArgb(45, 45, 48);
        public override Color ToolStripDropDownBackground => Color.FromArgb(45, 45, 48);
        public override Color SeparatorDark => Color.FromArgb(51, 51, 55);
        public override Color SeparatorLight => Color.FromArgb(51, 51, 55);
    }

    /// <summary>
    /// ListView subclass that disables the Windows visual-styles theme
    /// so the native renderer does not paint hot-track overlays on top
    /// of owner-draw output, suppresses native background erase, and
    /// manually tracks the hot (hovered) item to avoid partial-repaint
    /// artefacts that make items disappear on mouse hover.
    /// </summary>
    internal class DoubleBufferedListView : ListView
    {
        private const int WM_ERASEBKGND = 0x0014;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_MOUSELEAVE = 0x02A3;
        private const int LVM_FIRST = 0x1000;
        private const int LVM_HITTEST = LVM_FIRST + 18;

        private int _hotIndex = -1;

        public DoubleBufferedListView()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            NativeMethods.SetWindowTheme(Handle, "", "");
        }

        protected override void WndProc(ref Message m)
        {
            // Block native background erase — our owner-draw handlers paint
            // every pixel, so letting the control erase first causes flicker.
            if (m.Msg == WM_ERASEBKGND)
            {
                m.Result = (IntPtr)1;
                return;
            }

            // On mouse move, figure out which item the cursor is over and
            // invalidate only the rows that changed (old hot row + new hot row).
            // This replaces the native hot-tracking that paints over our
            // owner-draw output with theme colours.
            if (m.Msg == WM_MOUSEMOVE)
            {
                var pt = PointToClient(Cursor.Position);
                int newHot = HitTestIndex(pt);
                if (newHot != _hotIndex)
                {
                    int oldHot = _hotIndex;
                    _hotIndex = newHot;
                    InvalidateItem(oldHot);
                    InvalidateItem(newHot);
                }
                // Still let the base handle mouse move for scrolling etc.
                base.WndProc(ref m);
                return;
            }

            if (m.Msg == WM_MOUSELEAVE)
            {
                int oldHot = _hotIndex;
                _hotIndex = -1;
                InvalidateItem(oldHot);
            }

            base.WndProc(ref m);
        }

        private int HitTestIndex(Point clientPoint)
        {
            var lhi = new LVHITTESTINFO
            {
                pt = clientPoint
            };
            int size = System.Runtime.InteropServices.Marshal.SizeOf<LVHITTESTINFO>();
            IntPtr ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
            try
            {
                System.Runtime.InteropServices.Marshal.StructureToPtr(lhi, ptr, false);
                NativeMethods.SendMessage(Handle, LVM_HITTEST, IntPtr.Zero, ptr);
                lhi = System.Runtime.InteropServices.Marshal.PtrToStructure<LVHITTESTINFO>(ptr);
                return lhi.iItem;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
            }
        }

        private void InvalidateItem(int index)
        {
            if (index < 0 || index >= Items.Count) return;
            var rect = Items[index].Bounds;
            // Expand to full row width
            rect = new Rectangle(0, rect.Y, ClientSize.Width, rect.Height);
            Invalidate(rect);
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct LVHITTESTINFO
        {
            public Point pt;
            public uint flags;
            public int iItem;
            public int iSubItem;
            public int iGroup;
        }
    }
}
