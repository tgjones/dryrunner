using System;
using System.IO;

namespace DryRunner
{
	public class TestSiteManager
	{
		private readonly TestSiteDeployer _deployer;
		private readonly TestSiteServer _server;

		public TestSiteManager(int port, string projectName)
		{
			string siteRoot = GetPathRelativeToCurrentAssemblyPath(@"..\..\..\" + projectName);
			if (!Directory.Exists(siteRoot))
				throw new Exception("A project with name '" + projectName + "' could not be found.");

			_deployer = new TestSiteDeployer(siteRoot, projectName);
			_server = new TestSiteServer(port, _deployer.TestSitePath);
		}

		public void Start()
		{
			_deployer.Deploy();
			_server.Start();
		}

		public void Stop()
		{
			_server.Stop();
		}

		private static string GetPathRelativeToCurrentAssemblyPath(string relativePath)
		{
			string asmFilePath = new Uri(typeof(TestSiteDeployer).Assembly.CodeBase).LocalPath;
			string asmPath = Path.GetDirectoryName(asmFilePath);
			string fullPath = Path.Combine(asmPath, relativePath);
			return Path.GetFullPath(fullPath);
		}
	}
}