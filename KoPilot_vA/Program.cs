namespace KoPilot_vA
{
    internal static class Program
    {
        /// <summary>
        /// The application icon, extracted once from the running executable.
        /// Every form in the app should set <c>Icon = Program.AppIcon</c>.
        /// </summary>
        public static Icon? AppIcon { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Load the application icon from the executable for reuse across all forms
            try
            {
                var exePath = Environment.ProcessPath ?? Application.ExecutablePath;
                AppIcon = Icon.ExtractAssociatedIcon(exePath);
            }
            catch { }

            // Catch unhandled exceptions on the UI thread
            Application.ThreadException += (_, e) =>
            {
                ShowCrashDialog(e.Exception);
            };

            // Catch unhandled exceptions on background / finalizer threads
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                    ShowCrashDialog(ex);
            };

            // Ensure UI-thread exceptions route through ThreadException rather than crashing
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            using var openForm = new OpenForm();
            if (openForm.ShowDialog() != DialogResult.OK)
                return;

            var mainForm = new Form1();

            if (openForm.CreateNewProject)
            {
                mainForm.RequestNewProjectOnLoad = true;
            }
            else if (!string.IsNullOrEmpty(openForm.SelectedProjectPath))
            {
                mainForm.ProjectToLoadOnStart = openForm.SelectedProjectPath;
            }

            Application.Run(mainForm);
        }

        private static void ShowCrashDialog(Exception ex)
        {
            try
            {
                var message =
                    $"An unexpected error occurred:\n\n" +
                    $"{ex.GetType().Name}: {ex.Message}\n\n" +
                    $"{ex.StackTrace}";

                MessageBox.Show(message, "KoPilot IDE - Unhandled Exception",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                // Last resort — if even MessageBox fails, do nothing
            }
        }
    }
}