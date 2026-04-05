namespace KoPilot_vA
{
    public partial class AIEndPointSettings : Form
    {
        public string EndpointUrl => txtEndpointUrl.Text.Trim();
        public int MaxTokens => (int)nudMaxTokens.Value;
        public int ContextLength => (int)nudContextLength.Value;
        public double Temperature => (double)nudTemperature.Value;
        public string SystemPrompt => txtSystemPrompt.Text;
        public bool IsEnabled => chkEnabled.Checked;

        public AIEndPointSettings()
        {
            InitializeComponent();
        }

        public void ApplyFrom(string url, int maxTokens, int contextLength, double temperature, string systemPrompt, bool enabled)
        {
            txtEndpointUrl.Text = url;
            nudMaxTokens.Value = Math.Clamp(maxTokens, (int)nudMaxTokens.Minimum, (int)nudMaxTokens.Maximum);
            nudContextLength.Value = Math.Clamp(contextLength, (int)nudContextLength.Minimum, (int)nudContextLength.Maximum);
            nudTemperature.Value = (decimal)temperature;
            txtSystemPrompt.Text = systemPrompt;
            chkEnabled.Checked = enabled;
        }

        private void btnOk_Click(object? sender, EventArgs e)
        {
            if (chkEnabled.Checked && string.IsNullOrWhiteSpace(EndpointUrl))
            {
                MessageBox.Show("Please enter an endpoint URL or disable the AI endpoint.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnTest_Click(object? sender, EventArgs e)
        {
            TestConnectionAsync();
        }

        private async void TestConnectionAsync()
        {
            if (string.IsNullOrWhiteSpace(EndpointUrl))
            {
                MessageBox.Show("Please enter an endpoint URL first.", "Test Connection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnTest.Enabled = false;
            btnTest.Text = "Testing...";

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var modelUrl = EndpointUrl.TrimEnd('/');
                if (!modelUrl.EndsWith("/api/v1/model"))
                    modelUrl += "/api/v1/model";

                var response = await client.GetAsync(modelUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Connection successful!\n\nResponse: {content}", "Test Connection",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Connection failed. Status: {response.StatusCode}", "Test Connection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}", "Test Connection",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnTest.Enabled = true;
                btnTest.Text = "Test Connection";
            }
        }
    }
}
