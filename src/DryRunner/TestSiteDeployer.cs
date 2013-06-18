using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace DryRunner
{
	public class TestSiteDeployer
	{
		private readonly string _siteRoot;
		private readonly string _projectName;

		public string TestSitePath
		{
			get { return Path.Combine(_siteRoot, @"obj\Test\Package\PackageTmp"); }
		}

		public TestSiteDeployer(string siteRoot, string projectName)
		{
			_siteRoot = siteRoot;
			_projectName = projectName;
		}

	  public void Deploy ()
	  {
	    var buildManager = BuildManager.DefaultBuildManager;
	    buildManager.ResetCaches();

	    var recordingEventRedirector = new RecordingEventRedirector();
	    var forwardingLogger = new ConfigurableForwardingLogger { BuildEventRedirector = recordingEventRedirector };
	    var consoleLogger = new ConsoleLogger(LoggerVerbosity.Quiet);
	    var buildParameters = new BuildParameters { Loggers = new ILogger[] { consoleLogger, forwardingLogger } };
	    buildManager.BeginBuild(buildParameters);

	    // 1) Clean previous deployment.
      // 2) Do a build of the website project with the "Package" target. This will copy all
      //    the necessary website files into a directory similar to the following:
      //    MyProject/obj/Test/Package/PackageTmp
	    var result = BuildRequest(buildManager, "Clean", "Package");

	    buildManager.EndBuild();

	    if (result.OverallResult != BuildResultCode.Success || !Directory.Exists(TestSitePath))
	    {
	      var message = "Build failed! See build output and ensure that you have a Test build configuration."
	                    + Environment.NewLine + Environment.NewLine
	                    + recordingEventRedirector.GetJoinedBuildMessages();
	      throw new BuildFailedException(message);
	    }
	  }

	  private BuildResult BuildRequest (BuildManager buildManager, params string[] targetsToBuild)
	  {
	    var projectFilePath = Path.Combine(_siteRoot, _projectName + ".csproj");
	    var globalProperties = new Dictionary<string, string> { { "Configuration", "Test" } };
	    var requestData = new BuildRequestData(projectFilePath, globalProperties, null, targetsToBuild, null);

	    return buildManager.BuildRequest(requestData);
	  }
	}
}