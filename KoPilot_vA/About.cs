using System.Reflection;

namespace KoPilot_vA
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            PopulateVersionInfo();
            PopulateDetails();
        }

        private void PopulateVersionInfo()
        {
            var asm = Assembly.GetExecutingAssembly();
            var ver = asm.GetName().Version ?? new Version(1, 0, 0, 0);
            lblVersion.Text = $"Version {ver.Major}.{ver.Minor}.{ver.Build}  \u2022  .NET {Environment.Version}  \u2022  {(Environment.Is64BitProcess ? "64-bit" : "32-bit")}";
            lblCopyright.Text = $"\u00A9 {DateTime.Now.Year} Michael Sullender  \u2022  Control Feed  \u2022  All rights reserved.";
        }

        private void PopulateDetails()
        {
            txtDetails.Clear();

            AppendHeading("About KoPilot IDE");
            AppendBody(
                "KoPilot is a lightweight, AI-powered Integrated Development Environment " +
                "built specifically for C# development. It combines the essential features " +
                "of a modern code editor with the intelligence of a local AI assistant, " +
                "enabling developers to write, build, run, and iterate on .NET projects " +
                "without leaving a single application.\n");

            AppendHeading("Created by");
            AppendBody(
                "Michael Sullender \u2014 developer, designer, and architect of KoPilot. " +
                "This application was conceived, designed, and brought to life through a " +
                "collaborative human\u2013AI development process, demonstrating what is possible " +
                "when a developer partners with artificial intelligence at every stage of " +
                "the software lifecycle.\n");

            AppendHeading("Designed by Control Feed");
            AppendBody(
                "Control Feed is Michael Sullender\u2019s AI-based organization dedicated to " +
                "exploring the intersection of artificial intelligence and software engineering. " +
                "Control Feed focuses on building tools, workflows, and applications that " +
                "leverage AI to amplify human creativity and productivity. KoPilot IDE is a " +
                "flagship product of this vision \u2014 an IDE that was itself built with AI " +
                "assistance and embeds AI directly into the development experience.\n");

            AppendHeading("Built with AI");
            AppendBody(
                "KoPilot IDE was developed using an AI-assisted workflow from the ground up. " +
                "Every major component \u2014 the editor, the project system, the build pipeline, " +
                "syntax highlighting, IntelliSense, and the AI chat panel \u2014 was designed and " +
                "implemented through an iterative conversation between the developer and an " +
                "AI coding assistant. This approach allowed rapid prototyping, architectural " +
                "exploration, and feature development at a pace that would be difficult to " +
                "achieve through traditional methods alone.\n");

            AppendHeading("Key Features");
            AppendBullet("Multi-file tabbed code editor with Consolas font and dark theme");
            AppendBullet("Roslyn-powered syntax highlighting with Visual Studio\u2013style coloring");
            AppendBullet("IntelliSense auto-completion with cross-file type awareness");
            AppendBullet("Integrated project system with .kopilot project files");
            AppendBullet("Build, Run, and Clean via the .NET CLI (dotnet)");
            AppendBullet("MSBuild error parsing with click-to-navigate error list");
            AppendBullet("Runtime exception detection with automatic source navigation");
            AppendBullet("Line number gutter with active line highlighting");
            AppendBullet("AI chat panel powered by KoboldCpp for local LLM inference");
            AppendBullet("AI-driven code generation with automatic file creation and updates");
            AppendBullet("Chat history with export and import support");
            AppendBullet("File Explorer with tree view, context menus, and project scanning");
            AppendBullet("Recent projects list with persistent storage");
            AppendBullet("Configurable editor settings (font, word wrap, .NET SDK path)");
            AppendBullet("Configurable AI endpoint (URL, tokens, context length, temperature)");
            AppendBody("");

            AppendHeading("Technology Stack");
            AppendBullet(".NET 8 with Windows Forms (WinForms)");
            AppendBullet("C# 12 with modern language features");
            AppendBullet("Microsoft.CodeAnalysis (Roslyn) for syntax classification and IntelliSense");
            AppendBullet("KoboldCpp REST API for local AI model inference");
            AppendBullet("System.Text.Json for serialization");
            AppendBullet("Custom dark theme rendering throughout the UI");
            AppendBody("");

            AppendHeading("Philosophy");
            AppendBody(
                "KoPilot IDE represents a new approach to tool building: rather than AI being " +
                "an afterthought bolted onto an existing product, it is woven into the DNA of " +
                "both the application and its creation process. The goal is to demonstrate that " +
                "AI-assisted development is not just viable \u2014 it is the future of how we " +
                "build software.\n");

            AppendHeading("Acknowledgments");
            AppendBody(
                "Special thanks to the open-source communities behind .NET, Roslyn, and " +
                "KoboldCpp, whose work makes projects like KoPilot possible. And to every AI " +
                "model that contributed lines of code, suggested architectures, and helped debug " +
                "at 2 AM \u2014 this project is as much yours as it is mine.\n");

            // Scroll back to top
            txtDetails.SelectionStart = 0;
            txtDetails.ScrollToCaret();
        }

        private void AppendHeading(string text)
        {
            txtDetails.SelectionStart = txtDetails.TextLength;
            txtDetails.SelectionLength = 0;
            txtDetails.SelectionFont = new Font("Segoe UI", 11.5f, FontStyle.Bold);
            txtDetails.SelectionColor = Color.FromArgb(0, 122, 204);
            txtDetails.AppendText(text + "\n");

            // Thin underline via a dimmed separator line
            txtDetails.SelectionStart = txtDetails.TextLength;
            txtDetails.SelectionFont = new Font("Segoe UI", 4f);
            txtDetails.SelectionColor = Color.FromArgb(62, 62, 66);
            txtDetails.AppendText("\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\n");
        }

        private void AppendBody(string text)
        {
            txtDetails.SelectionStart = txtDetails.TextLength;
            txtDetails.SelectionLength = 0;
            txtDetails.SelectionFont = new Font("Segoe UI", 10f);
            txtDetails.SelectionColor = Color.FromArgb(210, 210, 210);
            txtDetails.AppendText(text + "\n");
        }

        private void AppendBullet(string text)
        {
            txtDetails.SelectionStart = txtDetails.TextLength;
            txtDetails.SelectionLength = 0;
            txtDetails.SelectionFont = new Font("Segoe UI", 10f);
            txtDetails.SelectionColor = Color.FromArgb(0, 122, 204);
            txtDetails.AppendText("  \u25B8  ");
            txtDetails.SelectionColor = Color.FromArgb(200, 200, 200);
            txtDetails.AppendText(text + "\n");
        }
    }
}
