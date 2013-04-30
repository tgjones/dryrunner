using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace DryRunner
{
	public class TestSiteServer
	{
		private readonly int _port;
		private readonly string _siteRoot;
		private Process _process;
		private ManualResetEventSlim _manualResetEvent;

		public TestSiteServer(int port, string siteRoot)
		{
			_port = port;
			_siteRoot = siteRoot;
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
			var applicationHostPath = CreateApplicationHost();

			var startInfo = new ProcessStartInfo
			{
				WindowStyle = ProcessWindowStyle.Normal,
				ErrorDialog = true,
				LoadUserProfile = true,
				CreateNoWindow = false,
				UseShellExecute = false,
				Arguments = string.Format("/config:\"{0}\"", applicationHostPath)
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

		private string CreateApplicationHost()
		{
			var applicationHostConfigStream = typeof (TestSiteServer).Assembly.GetManifestResourceStream(
				typeof (TestSiteServer), "applicationHost.config");
			string applicationHost;
			using (var reader = new StreamReader(applicationHostConfigStream))
				applicationHost = reader.ReadToEnd();
			applicationHost = applicationHost.Replace("{{PORT}}", _port.ToString());
			applicationHost = applicationHost.Replace("{{PATH}}", _siteRoot);

			string applicationHostPath = Path.GetFullPath("applicationHost.config");
			File.WriteAllText(applicationHostPath, applicationHost);
			return applicationHostPath;
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