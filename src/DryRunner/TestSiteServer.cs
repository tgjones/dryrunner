using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using DryRunner.Options;

namespace DryRunner
{
    /// <summary>
    /// Hosts the test site in IISExpress.
    /// </summary>
    public class TestSiteServer
    {
        private readonly string _physicalSitePath;
        private readonly TestSiteServerOptions _options;

        private Process _process;
        private ManualResetEventSlim _manualResetEvent;

        /// <summary>
        /// Creates a new test site server that hosts the application located on the given <paramref name="physicalSitePath"/> using 
        /// options defined in <paramref name="options"/>.
        /// </summary>
        public TestSiteServer(string physicalSitePath, TestSiteServerOptions options)
        {
            _physicalSitePath = physicalSitePath;
            _options = options;
        }

        /// <summary>
        /// Starts the IISExpress process that hosts the test site.
        /// </summary>
        public void Start()
        {
            _manualResetEvent = new ManualResetEventSlim(false);

            var thread = new Thread(StartIisExpress) { IsBackground = true };
            thread.Start();

            if (!_manualResetEvent.Wait(15000))
                throw new Exception("Could not start IIS Express");

            _manualResetEvent.Dispose();
        }

        private void StartIisExpress()
        {
            var applicationHostConfig = CreateApplicationHostConfig();

            var applicationHostPath = Path.GetFullPath("applicationHost.config");
            File.WriteAllText(applicationHostPath, applicationHostConfig);

            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Minimized,
                CreateNoWindow = !_options.ShowIisExpressWindow,
                Arguments = string.Format("/config:\"{0}\" /systray:true", applicationHostPath)
            };

            var programfiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            startInfo.FileName = programfiles + "\\IIS Express\\iisexpress.exe";

            try
            {
                _process = new Process { StartInfo = startInfo };

                _process.Start();
                _manualResetEvent.Set();
                _process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error starting IIS Express: " + ex);
                _process.CloseMainWindow();
                _process.Dispose();
            }
        }

        private string CreateApplicationHostConfig()
        {
            var applicationHostConfig = new StringBuilder(GetApplicationHostConfigTemplate());
            applicationHostConfig
                .Replace("{{PORT}}", _options.Port.ToString())
                .Replace("{{PHYSICAL_PATH}}", _physicalSitePath)
                .Replace("{{APPLICATION_PATH}}", _options.ApplicationPath)
                .Replace("{{WINDOWS_AUTHENTICATION_ENABLED}}", _options.EnableWindowsAuthentication ? "true" : "false");

            // There must always be a default application. So if we do not deploy to "/" we uncomment our dummy default application.
            // This default application gets served from a new directory created inside the physical path of our site (to avoid access rights issues).
            if (_options.ApplicationPath != "/")
            {
                var defaultApplicationPhysicalPath = Path.Combine(_physicalSitePath, "dummy-default-application");
                Directory.CreateDirectory(defaultApplicationPhysicalPath);

                applicationHostConfig
                    .Replace("{{DEFAULT_APPLICATION_PHYSICAL_PATH}}", defaultApplicationPhysicalPath)
                    .Replace("<!--{{DEFAULT_APPLICATION_COMMENT}}", string.Empty)
                    .Replace("{{DEFAULT_APPLICATION_COMMENT}}-->", string.Empty);
            }

            return applicationHostConfig.ToString();
        }

        private string GetApplicationHostConfigTemplate()
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(typeof(TestSiteServer), "applicationHost.config"))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        /// <summary>
        /// Stops the IISExpress process that hosts the test site.
        /// </summary>
        public void Stop()
        {
            if (_process == null)
                return;

            PostMessage(new HandleRef(_process, _process.MainWindowHandle), WM_KEYDOWN, VK_Q, IntPtr.Zero);
            _process.WaitForExit(5000);
            if (!_process.HasExited)
                _process.Kill();
        }

        private const int WM_KEYDOWN = 0x100;
        private static readonly IntPtr VK_Q = new IntPtr(0x51);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        static extern bool PostMessage(HandleRef hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}