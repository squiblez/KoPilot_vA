namespace KoPilot_vA
{
    partial class AIEndPointSettings
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
            txtEndpointUrl = new TextBox();
            nudMaxTokens = new NumericUpDown();
            nudContextLength = new NumericUpDown();
            nudTemperature = new NumericUpDown();
            txtSystemPrompt = new TextBox();
            chkEnabled = new CheckBox();
            btnTest = new Button();
            btnOk = new Button();
            btnCancel = new Button();

            ((System.ComponentModel.ISupportInitialize)nudMaxTokens).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudContextLength).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudTemperature).BeginInit();
            SuspendLayout();

            // Enabled
            chkEnabled.Text = "Enable AI Endpoint";
            chkEnabled.ForeColor = Color.White;
            chkEnabled.Location = new Point(20, 15);
            chkEnabled.AutoSize = true;
            chkEnabled.Checked = true;

            // Endpoint URL
            var lblUrl = new Label { Text = "KoboldCpp Endpoint URL:", Location = new Point(20, 50), AutoSize = true, ForeColor = Color.White };
            txtEndpointUrl.BackColor = Color.FromArgb(30, 30, 30);
            txtEndpointUrl.ForeColor = Color.White;
            txtEndpointUrl.Location = new Point(20, 75);
            txtEndpointUrl.Size = new Size(350, 27);
            txtEndpointUrl.Text = "http://192.168.20.227:5001";

            // Test button
            btnTest.FlatStyle = FlatStyle.Flat;
            btnTest.BackColor = Color.FromArgb(63, 63, 70);
            btnTest.ForeColor = Color.White;
            btnTest.Location = new Point(380, 74);
            btnTest.Size = new Size(110, 28);
            btnTest.Text = "Test Connection";
            btnTest.Click += btnTest_Click;

            // Max Tokens
            var lblTokens = new Label { Text = "Max Tokens:", Location = new Point(20, 115), AutoSize = true, ForeColor = Color.White };
            nudMaxTokens.BackColor = Color.FromArgb(30, 30, 30);
            nudMaxTokens.ForeColor = Color.White;
            nudMaxTokens.Location = new Point(20, 140);
            nudMaxTokens.Size = new Size(120, 27);
            nudMaxTokens.Minimum = 16;
            nudMaxTokens.Maximum = 32768;
            nudMaxTokens.Value = 4096;

            // Context Length
            var lblContext = new Label { Text = "Context Length:", Location = new Point(170, 115), AutoSize = true, ForeColor = Color.White };
            nudContextLength.BackColor = Color.FromArgb(30, 30, 30);
            nudContextLength.ForeColor = Color.White;
            nudContextLength.Location = new Point(170, 140);
            nudContextLength.Size = new Size(120, 27);
            nudContextLength.Minimum = 1024;
            nudContextLength.Maximum = 1000000;
            nudContextLength.Increment = 1000;
            nudContextLength.Value = 250000;

            // Temperature
            var lblTemp = new Label { Text = "Temperature:", Location = new Point(320, 115), AutoSize = true, ForeColor = Color.White };
            nudTemperature.BackColor = Color.FromArgb(30, 30, 30);
            nudTemperature.ForeColor = Color.White;
            nudTemperature.Location = new Point(320, 140);
            nudTemperature.Size = new Size(100, 27);
            nudTemperature.Minimum = 0;
            nudTemperature.Maximum = 2;
            nudTemperature.DecimalPlaces = 2;
            nudTemperature.Increment = 0.05m;
            nudTemperature.Value = 0.7m;

            // System Prompt
            var lblPrompt = new Label { Text = "System Prompt:", Location = new Point(20, 180), AutoSize = true, ForeColor = Color.White };
            txtSystemPrompt.BackColor = Color.FromArgb(30, 30, 30);
            txtSystemPrompt.ForeColor = Color.White;
            txtSystemPrompt.Location = new Point(20, 205);
            txtSystemPrompt.Size = new Size(470, 120);
            txtSystemPrompt.Multiline = true;
            txtSystemPrompt.ScrollBars = ScrollBars.Vertical;
            txtSystemPrompt.Text = "You are a helpful C# coding assistant. Help the user write, debug, and improve their code.";

            // OK / Cancel
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.BackColor = Color.FromArgb(0, 122, 204);
            btnOk.ForeColor = Color.White;
            btnOk.Location = new Point(320, 340);
            btnOk.Size = new Size(80, 32);
            btnOk.Text = "OK";
            btnOk.Click += btnOk_Click;

            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.BackColor = Color.FromArgb(63, 63, 70);
            btnCancel.ForeColor = Color.White;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(410, 340);
            btnCancel.Size = new Size(80, 32);
            btnCancel.Text = "Cancel";

            // AIEndPointSettings Form
            AcceptButton = btnOk;
            CancelButton = btnCancel;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(45, 45, 48);
            ClientSize = new Size(510, 390);
            Controls.Add(chkEnabled);
            Controls.Add(lblUrl);
            Controls.Add(txtEndpointUrl);
            Controls.Add(btnTest);
            Controls.Add(lblTokens);
            Controls.Add(nudMaxTokens);
            Controls.Add(lblContext);
            Controls.Add(nudContextLength);
            Controls.Add(lblTemp);
            Controls.Add(nudTemperature);
            Controls.Add(lblPrompt);
            Controls.Add(txtSystemPrompt);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AIEndPointSettings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "AI Endpoint Settings";
            if (Program.AppIcon != null) Icon = Program.AppIcon;

            ((System.ComponentModel.ISupportInitialize)nudMaxTokens).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudContextLength).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudTemperature).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtEndpointUrl;
        private NumericUpDown nudMaxTokens;
        private NumericUpDown nudContextLength;
        private NumericUpDown nudTemperature;
        private TextBox txtSystemPrompt;
        private CheckBox chkEnabled;
        private Button btnTest;
        private Button btnOk;
        private Button btnCancel;
    }
}
