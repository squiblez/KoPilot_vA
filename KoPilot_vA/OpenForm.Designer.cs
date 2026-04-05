namespace KoPilot_vA
{
    partial class OpenForm
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
            SuspendLayout();

            // ── Accent stripe ──
            var panelAccent = new Panel();
            panelAccent.Dock = DockStyle.Top;
            panelAccent.Height = 5;
            panelAccent.BackColor = Color.FromArgb(0, 122, 204);

            // ── Left branding panel ──
            panelBrand = new Panel();
            panelBrand.Dock = DockStyle.Left;
            panelBrand.Width = 280;
            panelBrand.BackColor = Color.FromArgb(27, 27, 28);
            panelBrand.Padding = new Padding(28, 40, 28, 20);

            lblAppName = new Label();
            lblAppName.Text = "KoPilot IDE";
            lblAppName.Font = new Font("Segoe UI", 28f, FontStyle.Bold);
            lblAppName.ForeColor = Color.FromArgb(0, 122, 204);
            lblAppName.AutoSize = true;
            lblAppName.Location = new Point(28, 40);

            lblTagline = new Label();
            lblTagline.Text = "AI-Powered C#\nDevelopment Environment";
            lblTagline.Font = new Font("Segoe UI", 10.5f, FontStyle.Italic);
            lblTagline.ForeColor = Color.FromArgb(150, 150, 150);
            lblTagline.AutoSize = true;
            lblTagline.Location = new Point(30, 92);

            lblVersion = new Label();
            lblVersion.Font = new Font("Segoe UI", 8.5f);
            lblVersion.ForeColor = Color.FromArgb(100, 100, 100);
            lblVersion.AutoSize = true;
            lblVersion.Location = new Point(30, 148);

            lblEndpoint = new Label();
            lblEndpoint.Font = new Font("Segoe UI", 8f);
            lblEndpoint.ForeColor = Color.FromArgb(80, 80, 80);
            lblEndpoint.AutoSize = true;
            lblEndpoint.MaximumSize = new Size(220, 0);
            lblEndpoint.Location = new Point(30, 172);

            // Separator line in brand panel
            var brandSep = new Panel();
            brandSep.BackColor = Color.FromArgb(50, 50, 52);
            brandSep.Size = new Size(220, 1);
            brandSep.Location = new Point(30, 205);

            lblCredit = new Label();
            lblCredit.Text = "by Michael Sullender\nDesigned by Control Feed";
            lblCredit.Font = new Font("Segoe UI", 8f);
            lblCredit.ForeColor = Color.FromArgb(80, 80, 80);
            lblCredit.AutoSize = true;
            lblCredit.Location = new Point(30, 218);

            // Action buttons in brand panel
            btnNew = new Button();
            btnNew.Text = "\u2795  New Project";
            btnNew.FlatStyle = FlatStyle.Flat;
            btnNew.FlatAppearance.BorderColor = Color.FromArgb(0, 122, 204);
            btnNew.BackColor = Color.FromArgb(0, 122, 204);
            btnNew.ForeColor = Color.White;
            btnNew.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnNew.Size = new Size(224, 38);
            btnNew.Location = new Point(28, 280);
            btnNew.Cursor = Cursors.Hand;
            btnNew.Click += BtnNew_Click;

            btnBrowse = new Button();
            btnBrowse.Text = "\uD83D\uDCC2  Open Project...";
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.FlatAppearance.BorderColor = Color.FromArgb(62, 62, 66);
            btnBrowse.BackColor = Color.FromArgb(45, 45, 48);
            btnBrowse.ForeColor = Color.FromArgb(200, 200, 200);
            btnBrowse.Font = new Font("Segoe UI", 9.5f);
            btnBrowse.Size = new Size(224, 34);
            btnBrowse.Location = new Point(28, 326);
            btnBrowse.Cursor = Cursors.Hand;
            btnBrowse.Click += BtnBrowse_Click;

            btnQuit = new Button();
            btnQuit.Text = "Quit";
            btnQuit.FlatStyle = FlatStyle.Flat;
            btnQuit.FlatAppearance.BorderSize = 0;
            btnQuit.BackColor = Color.FromArgb(27, 27, 28);
            btnQuit.ForeColor = Color.FromArgb(120, 120, 120);
            btnQuit.Font = new Font("Segoe UI", 9f);
            btnQuit.Size = new Size(224, 30);
            btnQuit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnQuit.Location = new Point(28, 440);
            btnQuit.Cursor = Cursors.Hand;
            btnQuit.Click += BtnQuit_Click;

            panelBrand.Controls.Add(lblAppName);
            panelBrand.Controls.Add(lblTagline);
            panelBrand.Controls.Add(lblVersion);
            panelBrand.Controls.Add(lblEndpoint);
            panelBrand.Controls.Add(brandSep);
            panelBrand.Controls.Add(lblCredit);
            panelBrand.Controls.Add(btnNew);
            panelBrand.Controls.Add(btnBrowse);
            panelBrand.Controls.Add(btnQuit);

            // ── Vertical separator ──
            var panelVSep = new Panel();
            panelVSep.Dock = DockStyle.Left;
            panelVSep.Width = 1;
            panelVSep.BackColor = Color.FromArgb(50, 50, 52);

            // ── Right content panel (recent projects) ──
            panelContent = new Panel();
            panelContent.Dock = DockStyle.Fill;
            panelContent.BackColor = Color.FromArgb(37, 37, 38);
            panelContent.Padding = new Padding(24, 20, 24, 20);

            lblRecentTitle = new Label();
            lblRecentTitle.Text = "Recent Projects";
            lblRecentTitle.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
            lblRecentTitle.ForeColor = Color.FromArgb(220, 220, 220);
            lblRecentTitle.Dock = DockStyle.Top;
            lblRecentTitle.Height = 36;
            lblRecentTitle.TextAlign = ContentAlignment.BottomLeft;

            lblRecentSub = new Label();
            lblRecentSub.Text = "Double-click a project to open it, or use the buttons on the left.";
            lblRecentSub.Font = new Font("Segoe UI", 9f);
            lblRecentSub.ForeColor = Color.FromArgb(130, 130, 130);
            lblRecentSub.Dock = DockStyle.Top;
            lblRecentSub.Height = 26;
            lblRecentSub.TextAlign = ContentAlignment.TopLeft;

            // List of recent projects
            lstRecent = new ListView();
            lstRecent.Dock = DockStyle.Fill;
            lstRecent.BackColor = Color.FromArgb(30, 30, 30);
            lstRecent.ForeColor = Color.FromArgb(220, 220, 220);
            lstRecent.Font = new Font("Segoe UI", 10f);
            lstRecent.BorderStyle = BorderStyle.None;
            lstRecent.View = View.Details;
            lstRecent.FullRowSelect = true;
            lstRecent.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lstRecent.MultiSelect = false;
            lstRecent.HideSelection = false;
            lstRecent.Columns.Add("Project", 180);
            lstRecent.Columns.Add("Location", 340);
            lstRecent.DoubleClick += LstRecent_DoubleClick;
            lstRecent.SelectedIndexChanged += LstRecent_SelectedIndexChanged;

            // "No recent projects" placeholder
            lblNoRecent = new Label();
            lblNoRecent.Text = "No recent projects.\n\nClick \"New Project\" to get started,\nor \"Open Project...\" to browse for an existing one.";
            lblNoRecent.Font = new Font("Segoe UI", 10.5f);
            lblNoRecent.ForeColor = Color.FromArgb(100, 100, 100);
            lblNoRecent.Dock = DockStyle.Fill;
            lblNoRecent.TextAlign = ContentAlignment.MiddleCenter;
            lblNoRecent.Visible = false;

            // Bottom button row in content panel
            panelButtons = new Panel();
            panelButtons.Dock = DockStyle.Bottom;
            panelButtons.Height = 44;
            panelButtons.BackColor = Color.FromArgb(37, 37, 38);

            btnOpen = new Button();
            btnOpen.Text = "Open Selected";
            btnOpen.FlatStyle = FlatStyle.Flat;
            btnOpen.BackColor = Color.FromArgb(0, 122, 204);
            btnOpen.ForeColor = Color.White;
            btnOpen.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnOpen.Size = new Size(130, 34);
            btnOpen.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnOpen.Location = new Point(380, 5);
            btnOpen.Enabled = false;
            btnOpen.Cursor = Cursors.Hand;
            btnOpen.Click += BtnOpen_Click;

            btnRemoveRecent = new Button();
            btnRemoveRecent.Text = "Remove";
            btnRemoveRecent.FlatStyle = FlatStyle.Flat;
            btnRemoveRecent.FlatAppearance.BorderColor = Color.FromArgb(62, 62, 66);
            btnRemoveRecent.BackColor = Color.FromArgb(45, 45, 48);
            btnRemoveRecent.ForeColor = Color.FromArgb(170, 170, 170);
            btnRemoveRecent.Font = new Font("Segoe UI", 9f);
            btnRemoveRecent.Size = new Size(80, 34);
            btnRemoveRecent.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnRemoveRecent.Location = new Point(292, 5);
            btnRemoveRecent.Cursor = Cursors.Hand;
            btnRemoveRecent.Click += BtnRemoveRecent_Click;

            panelButtons.Controls.Add(btnOpen);
            panelButtons.Controls.Add(btnRemoveRecent);

            panelContent.Controls.Add(lstRecent);
            panelContent.Controls.Add(lblNoRecent);
            panelContent.Controls.Add(panelButtons);
            panelContent.Controls.Add(lblRecentSub);
            panelContent.Controls.Add(lblRecentTitle);

            // ── Form ──
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 37, 38);
            ClientSize = new Size(800, 500);
            MinimumSize = new Size(700, 420);
            Controls.Add(panelContent);
            Controls.Add(panelVSep);
            Controls.Add(panelBrand);
            Controls.Add(panelAccent);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "OpenForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "KoPilot IDE";
            ShowInTaskbar = true;
            if (Program.AppIcon != null) Icon = Program.AppIcon;

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelBrand;
        private Label lblAppName;
        private Label lblTagline;
        private Label lblVersion;
        private Label lblEndpoint;
        private Label lblCredit;
        private Button btnNew;
        private Button btnBrowse;
        private Button btnQuit;

        private Panel panelContent;
        private Label lblRecentTitle;
        private Label lblRecentSub;
        private ListView lstRecent;
        private Label lblNoRecent;
        private Panel panelButtons;
        private Button btnOpen;
        private Button btnRemoveRecent;
    }
}