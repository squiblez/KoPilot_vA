namespace KoPilot_vA
{
    partial class Settings
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
            txtFontFamily = new TextBox();
            nudFontSize = new NumericUpDown();
            chkWordWrap = new CheckBox();
            chkLineNumbers = new CheckBox();
            txtDefaultPath = new TextBox();
            txtDotNetPath = new TextBox();
            btnBrowsePath = new Button();
            btnOk = new Button();
            btnCancel = new Button();

            ((System.ComponentModel.ISupportInitialize)nudFontSize).BeginInit();
            SuspendLayout();

            // Font Family
            var lblFont = new Label { Text = "Editor Font:", Location = new Point(20, 20), AutoSize = true, ForeColor = Color.White };
            txtFontFamily.BackColor = Color.FromArgb(30, 30, 30);
            txtFontFamily.ForeColor = Color.White;
            txtFontFamily.Location = new Point(20, 45);
            txtFontFamily.Size = new Size(200, 27);

            // Font Size
            var lblSize = new Label { Text = "Font Size:", Location = new Point(240, 20), AutoSize = true, ForeColor = Color.White };
            nudFontSize.BackColor = Color.FromArgb(30, 30, 30);
            nudFontSize.ForeColor = Color.White;
            nudFontSize.Location = new Point(240, 45);
            nudFontSize.Size = new Size(80, 27);
            nudFontSize.Minimum = 6;
            nudFontSize.Maximum = 48;
            nudFontSize.Value = 11;

            // Word Wrap
            chkWordWrap.Text = "Word Wrap";
            chkWordWrap.ForeColor = Color.White;
            chkWordWrap.Location = new Point(20, 85);
            chkWordWrap.AutoSize = true;

            // Line Numbers
            chkLineNumbers.Text = "Show Line Numbers (placeholder)";
            chkLineNumbers.ForeColor = Color.White;
            chkLineNumbers.Location = new Point(20, 115);
            chkLineNumbers.AutoSize = true;

            // Default Project Path
            var lblDefaultPath = new Label { Text = "Default Project Path:", Location = new Point(20, 150), AutoSize = true, ForeColor = Color.White };
            txtDefaultPath.BackColor = Color.FromArgb(30, 30, 30);
            txtDefaultPath.ForeColor = Color.White;
            txtDefaultPath.Location = new Point(20, 175);
            txtDefaultPath.Size = new Size(310, 27);

            btnBrowsePath.FlatStyle = FlatStyle.Flat;
            btnBrowsePath.BackColor = Color.FromArgb(63, 63, 70);
            btnBrowsePath.ForeColor = Color.White;
            btnBrowsePath.Location = new Point(340, 174);
            btnBrowsePath.Size = new Size(80, 28);
            btnBrowsePath.Text = "Browse...";
            btnBrowsePath.Click += btnBrowsePath_Click;

            // DotNet SDK Path
            var lblDotNet = new Label { Text = ".NET SDK/CLI Path:", Location = new Point(20, 215), AutoSize = true, ForeColor = Color.White };
            txtDotNetPath.BackColor = Color.FromArgb(30, 30, 30);
            txtDotNetPath.ForeColor = Color.White;
            txtDotNetPath.Location = new Point(20, 240);
            txtDotNetPath.Size = new Size(400, 27);

            // OK / Cancel
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.BackColor = Color.FromArgb(0, 122, 204);
            btnOk.ForeColor = Color.White;
            btnOk.Location = new Point(250, 290);
            btnOk.Size = new Size(80, 32);
            btnOk.Text = "OK";
            btnOk.Click += btnOk_Click;

            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.BackColor = Color.FromArgb(63, 63, 70);
            btnCancel.ForeColor = Color.White;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(340, 290);
            btnCancel.Size = new Size(80, 32);
            btnCancel.Text = "Cancel";

            // Settings Form
            AcceptButton = btnOk;
            CancelButton = btnCancel;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(45, 45, 48);
            ClientSize = new Size(450, 340);
            Controls.Add(lblFont);
            Controls.Add(txtFontFamily);
            Controls.Add(lblSize);
            Controls.Add(nudFontSize);
            Controls.Add(chkWordWrap);
            Controls.Add(chkLineNumbers);
            Controls.Add(lblDefaultPath);
            Controls.Add(txtDefaultPath);
            Controls.Add(btnBrowsePath);
            Controls.Add(lblDotNet);
            Controls.Add(txtDotNetPath);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Settings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            if (Program.AppIcon != null) Icon = Program.AppIcon;

            ((System.ComponentModel.ISupportInitialize)nudFontSize).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtFontFamily;
        private NumericUpDown nudFontSize;
        private CheckBox chkWordWrap;
        private CheckBox chkLineNumbers;
        private TextBox txtDefaultPath;
        private TextBox txtDotNetPath;
        private Button btnBrowsePath;
        private Button btnOk;
        private Button btnCancel;
    }
}
