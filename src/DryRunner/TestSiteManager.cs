using System;
using System.IO;

namespace DryRunner
{
	public class TestSiteManager
	{
		private readonly TestSiteDeployer _deployer;
		private readonly TestSiteServer _server;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestSiteManager" /> class.
    /// </summary>
    /// <param name="projectName">Name of the project, that is, the name of the <c>.csproj</c> for the web project without the file extension.</param>
    /// <param name="port">The port to use for the ISS Express instance.</param>
    /// <param name="applicationPath">The application path, defaults to the server root <c>"/"</c>.</param>
	  public TestSiteManager (string projectName, int port = 8888, string applicationPath = "/")
		{
			string siteRoot = GetPathRelativeToCurrentAssemblyPath(@"..\..\..\" + projectName);
			if (!Directory.Exists(siteRoot))
				throw new Exception("A project with name '" + projectName + "' could not be found.");

      _deployer = new TestSiteDeployer(siteRoot, projectName);
      _server = new TestSiteServer(_deployer.TestSitePath, port, applicationPath);
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