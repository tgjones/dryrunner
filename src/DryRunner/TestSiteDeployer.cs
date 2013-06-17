using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Execution;

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
	    buildManager.BeginBuild(new BuildParameters());

	    // Clean previous deployment.
	    BuildRequest(buildManager, "Clean");

	    // Do a build of the website project with the "Package" target. This will copy all
	    // the necessary website files into a directory similar to the following:
	    // MyProject/obj/Test/Package/PackageTmp
	    BuildRequest(buildManager, "Package");

	    buildManager.EndBuild();

	    if (!Directory.Exists(TestSitePath))
	      throw new Exception("Deployment package for Test build configuration not found; ensure you have a Test build configuration.");
	  }

	  private void BuildRequest (BuildManager buildManager, params string[] targetsToBuild)
	  {
	    var projectFilePath = Path.Combine(_siteRoot, _projectName + ".csproj");
	    var globalProperties = new Dictionary<string, string> { { "Configuration", "Test" } };
	    var requestData = new BuildRequestData(projectFilePath, globalProperties, null, targetsToBuild, null);

	    var result = buildManager.BuildRequest(requestData);
      if (result.OverallResult != BuildResultCode.Success)
        throw new Exception("Build failed; ensure you have a Test build configuration.");
	  }
	}
}