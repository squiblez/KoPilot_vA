namespace KoPilot_vA
{
    partial class NewProject
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
            txtProjectName = new TextBox();
            txtLocation = new TextBox();
            cmbLanguage = new ComboBox();
            btnBrowse = new Button();
            btnCreate = new Button();
            btnCancel = new Button();
            lblName = new Label();
            lblLocation = new Label();
            lblLanguage = new Label();
            SuspendLayout();

            // lblName
            lblName.AutoSize = true;
            lblName.ForeColor = Color.White;
            lblName.Location = new Point(20, 20);
            lblName.Name = "lblName";
            lblName.Text = "Project Name:";

            // txtProjectName
            txtProjectName.BackColor = Color.FromArgb(30, 30, 30);
            txtProjectName.ForeColor = Color.White;
            txtProjectName.Location = new Point(20, 45);
            txtProjectName.Name = "txtProjectName";
            txtProjectName.Size = new Size(390, 27);

            // lblLocation
            lblLocation.AutoSize = true;
            lblLocation.ForeColor = Color.White;
            lblLocation.Location = new Point(20, 85);
            lblLocation.Name = "lblLocation";
            lblLocation.Text = "Location:";

            // txtLocation
            txtLocation.BackColor = Color.FromArgb(30, 30, 30);
            txtLocation.ForeColor = Color.White;
            txtLocation.Location = new Point(20, 110);
            txtLocation.Name = "txtLocation";
            txtLocation.Size = new Size(300, 27);

            // btnBrowse
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.BackColor = Color.FromArgb(63, 63, 70);
            btnBrowse.ForeColor = Color.White;
            btnBrowse.Location = new Point(330, 109);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(80, 28);
            btnBrowse.Text = "Browse...";
            btnBrowse.Click += btnBrowse_Click;

            // lblLanguage
            lblLanguage.AutoSize = true;
            lblLanguage.ForeColor = Color.White;
            lblLanguage.Location = new Point(20, 150);
            lblLanguage.Name = "lblLanguage";
            lblLanguage.Text = "Language:";

            // cmbLanguage
            cmbLanguage.BackColor = Color.FromArgb(30, 30, 30);
            cmbLanguage.ForeColor = Color.White;
            cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLanguage.Location = new Point(20, 175);
            cmbLanguage.Name = "cmbLanguage";
            cmbLanguage.Size = new Size(200, 28);
            cmbLanguage.Items.AddRange(new object[] { "C#", "Plain Text" });

            // btnCreate
            btnCreate.FlatStyle = FlatStyle.Flat;
            btnCreate.BackColor = Color.FromArgb(0, 122, 204);
            btnCreate.ForeColor = Color.White;
            btnCreate.Location = new Point(230, 225);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(85, 32);
            btnCreate.Text = "Create";
            btnCreate.Click += btnCreate_Click;

            // btnCancel
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.BackColor = Color.FromArgb(63, 63, 70);
            btnCancel.ForeColor = Color.White;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(325, 225);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(85, 32);
            btnCancel.Text = "Cancel";

            // NewProject
            AcceptButton = btnCreate;
            CancelButton = btnCancel;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(45, 45, 48);
            ClientSize = new Size(435, 275);
            Controls.Add(lblName);
            Controls.Add(txtProjectName);
            Controls.Add(lblLocation);
            Controls.Add(txtLocation);
            Controls.Add(btnBrowse);
            Controls.Add(lblLanguage);
            Controls.Add(cmbLanguage);
            Controls.Add(btnCreate);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NewProject";
            StartPosition = FormStartPosition.CenterParent;
            Text = "New Project";
            if (Program.AppIcon != null) Icon = Program.AppIcon;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtProjectName;
        private TextBox txtLocation;
        private ComboBox cmbLanguage;
        private Button btnBrowse;
        private Button btnCreate;
        private Button btnCancel;
        private Label lblName;
        private Label lblLocation;
        private Label lblLanguage;
    }
}
