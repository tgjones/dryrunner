using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace DryRunner
{
	public class TestSiteServer
	{
        private readonly string _physicalSitePath;
        private readonly int _port;
        private readonly string _applicationPath;
	    private readonly bool _showIisExpressWindow;

	    private Process _process;
		private ManualResetEventSlim _manualResetEvent;

	    public TestSiteServer(string physicalSitePath, int port, string applicationPath, bool showIisExpressWindow)
	    {
	        _physicalSitePath = physicalSitePath;
	        _port = port;
	        _applicationPath = applicationPath;
	        _showIisExpressWindow = showIisExpressWindow;
	    }

	    public void Start()
		{
			_manualResetEvent = new ManualResetEventSlim(false);

			var thread = new Thread(StartIisExpress) { IsBackground = true };
			thread.Start();

			_manualResetEvent.Wait(5000);
			_manualResetEvent.Dispose();
		}

		private void StartIisExpress()
		{
			var applicationHostConfig = CreateApplicationHostConfig();

            var applicationHostPath = Path.GetFullPath("applicationHost.config");
            File.WriteAllText(applicationHostPath, applicationHostConfig);

			var startInfo = new ProcessStartInfo
			{
				WindowStyle = ProcessWindowStyle.Minimized,
				ErrorDialog = true,
				LoadUserProfile = true,
                CreateNoWindow = !_showIisExpressWindow,
				UseShellExecute = false,
                Arguments = string.Format("/config:\"{0}\" /systray:true", applicationHostPath)
			};

			var programfiles = string.IsNullOrEmpty(startInfo.EnvironmentVariables["ProgramFiles(x86)"])
				? startInfo.EnvironmentVariables["ProgramFiles"]
				: startInfo.EnvironmentVariables["ProgramFiles(x86)"];

			startInfo.FileName = programfiles + "\\IIS Express\\iisexpress.exe";

			try
			{
				_process = new Process { StartInfo = startInfo };

				_process.Start();
				_manualResetEvent.Set();
				_process.WaitForExit();
			}
			catch
			{
				_process.CloseMainWindow();
				_process.Dispose();
			}
		}

	    private string CreateApplicationHostConfig()
	    {
	        var applicationHostConfig = new StringBuilder(GetApplicationHostConfigTemplate());
	        applicationHostConfig
	            .Replace("{{PORT}}", _port.ToString())
	            .Replace("{{PHYSICAL_PATH}}", _physicalSitePath)
	            .Replace("{{APPLICATION_PATH}}", _applicationPath);

	        // There must always be a default application. So if we do not deploy to "/" we uncomment our dummy default application.
	        // This default application gets served from a new directory created inside the physical path of our site (to avoid access rights issues).
	        if (_applicationPath != "/")
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

	    public void Stop()
	    {
	        if (_process == null)
	            return;

	        _process.CloseMainWindow();
	        _process.WaitForExit(5000);
	        if (!_process.HasExited)
	            _process.Kill();
	    }
	}
}