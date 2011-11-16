using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Evaluation;
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

		public void Deploy()
		{
			// Do a build of the website project with the "Package" target. This will copy all
			// the necessary website files into a directory similar to the following:
			// MyProject/obj/Test/Package/PackageTmp
			BuildManager.DefaultBuildManager.Build(new BuildParameters(new ProjectCollection()),
				new BuildRequestData(Path.Combine(_siteRoot, _projectName + ".csproj"),
					new Dictionary<string, string> { { "Configuration", "Test" } },
					null, new[] { "Package" }, null));
		}
	}
}