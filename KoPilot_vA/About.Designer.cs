namespace KoPilot_vA
{
    partial class About
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

            // ── Accent stripe at top ──
            panelAccent = new Panel();
            panelAccent.Dock = DockStyle.Top;
            panelAccent.Height = 6;
            panelAccent.BackColor = Color.FromArgb(0, 122, 204);

            // ── Header panel (title + tagline + version) ──
            // Uses a TableLayoutPanel so labels flow vertically with auto-sized
            // rows and scale naturally with DPI / font size.
            panelHeader = new TableLayoutPanel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.AutoSize = true;
            panelHeader.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelHeader.BackColor = Color.FromArgb(30, 30, 30);
            panelHeader.Padding = new Padding(28, 14, 28, 10);
            panelHeader.ColumnCount = 1;
            panelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panelHeader.RowCount = 3;
            panelHeader.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panelHeader.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panelHeader.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            lblTitle = new Label();
            lblTitle.Text = "KoPilot IDE";
            lblTitle.Font = new Font("Segoe UI", 26f, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(0, 122, 204);
            lblTitle.AutoSize = true;
            lblTitle.Margin = new Padding(0, 0, 0, 2);

            lblTagline = new Label();
            lblTagline.Text = "AI-Powered Lightweight C# Development Environment";
            lblTagline.Font = new Font("Segoe UI", 11f, FontStyle.Italic);
            lblTagline.ForeColor = Color.FromArgb(170, 170, 170);
            lblTagline.AutoSize = true;
            lblTagline.Margin = new Padding(2, 0, 0, 2);

            lblVersion = new Label();
            lblVersion.Font = new Font("Segoe UI", 9f);
            lblVersion.ForeColor = Color.FromArgb(130, 130, 130);
            lblVersion.AutoSize = true;
            lblVersion.Margin = new Padding(2, 0, 0, 0);

            panelHeader.Controls.Add(lblTitle, 0, 0);
            panelHeader.Controls.Add(lblTagline, 0, 1);
            panelHeader.Controls.Add(lblVersion, 0, 2);

            // ── Separator ──
            panelSep1 = new Panel();
            panelSep1.Dock = DockStyle.Top;
            panelSep1.Height = 1;
            panelSep1.BackColor = Color.FromArgb(62, 62, 66);

            // ── Scrollable content area ──
            panelContent = new Panel();
            panelContent.Dock = DockStyle.Fill;
            panelContent.BackColor = Color.FromArgb(37, 37, 38);
            panelContent.Padding = new Padding(28, 16, 28, 16);

            txtDetails = new RichTextBox();
            txtDetails.Dock = DockStyle.Fill;
            txtDetails.BackColor = Color.FromArgb(37, 37, 38);
            txtDetails.ForeColor = Color.FromArgb(220, 220, 220);
            txtDetails.Font = new Font("Segoe UI", 10f);
            txtDetails.ReadOnly = true;
            txtDetails.BorderStyle = BorderStyle.None;
            txtDetails.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtDetails.DetectUrls = false;
            txtDetails.Cursor = Cursors.Default;

            panelContent.Controls.Add(txtDetails);

            // ── Bottom bar ──
            // Uses a TableLayoutPanel with two columns so the copyright text
            // and the Close button both position themselves via layout rather
            // than hard-coded pixel coordinates.
            panelBottom = new TableLayoutPanel();
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.AutoSize = true;
            panelBottom.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelBottom.BackColor = Color.FromArgb(30, 30, 30);
            panelBottom.Padding = new Padding(28, 12, 28, 12);
            panelBottom.ColumnCount = 2;
            panelBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panelBottom.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panelBottom.RowCount = 1;
            panelBottom.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            panelSep2 = new Panel();
            panelSep2.Dock = DockStyle.Top;
            panelSep2.Height = 1;
            panelSep2.BackColor = Color.FromArgb(62, 62, 66);

            lblCopyright = new Label();
            lblCopyright.Font = new Font("Segoe UI", 8.5f);
            lblCopyright.ForeColor = Color.FromArgb(130, 130, 130);
            lblCopyright.AutoSize = true;
            lblCopyright.Anchor = AnchorStyles.Left;
            lblCopyright.Margin = new Padding(0, 0, 8, 0);

            btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.BackColor = Color.FromArgb(0, 122, 204);
            btnClose.ForeColor = Color.White;
            btnClose.Font = new Font("Segoe UI", 9.5f);
            btnClose.AutoSize = true;
            btnClose.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnClose.Padding = new Padding(16, 2, 16, 2);
            btnClose.Anchor = AnchorStyles.Right;
            btnClose.DialogResult = DialogResult.OK;

            panelBottom.Controls.Add(lblCopyright, 0, 0);
            panelBottom.Controls.Add(btnClose, 1, 0);

            // ── Form ──
            AcceptButton = btnClose;
            CancelButton = btnClose;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(37, 37, 38);
            ClientSize = new Size(580, 620);
            MinimumSize = new Size(480, 400);
            Controls.Add(panelContent);
            Controls.Add(panelSep1);
            Controls.Add(panelHeader);
            Controls.Add(panelAccent);
            Controls.Add(panelSep2);
            Controls.Add(panelBottom);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "About";
            StartPosition = FormStartPosition.CenterParent;
            Text = "About KoPilot IDE";
            ShowInTaskbar = false;
            if (Program.AppIcon != null) Icon = Program.AppIcon;

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelAccent;
        private TableLayoutPanel panelHeader;
        private Label lblTitle;
        private Label lblTagline;
        private Label lblVersion;
        private Panel panelSep1;
        private Panel panelContent;
        private RichTextBox txtDetails;
        private TableLayoutPanel panelBottom;
        private Panel panelSep2;
        private Label lblCopyright;
        private Button btnClose;
    }
}