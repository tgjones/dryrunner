﻿using System;
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
	    var errorsOnlyRecorder = new RecordingEventRedirector();
	    var normalRecorder = new RecordingEventRedirector();
	    var loggers =
	        new ILogger[]
	        {
	            new ConsoleLogger(LoggerVerbosity.Quiet),
	            new ConfigurableForwardingLogger { BuildEventRedirector = errorsOnlyRecorder, Verbosity = LoggerVerbosity.Quiet },
	            new ConfigurableForwardingLogger { BuildEventRedirector = normalRecorder, Verbosity = LoggerVerbosity.Normal }
	        };

	    var result = Build(loggers);

	    if (result.OverallResult != BuildResultCode.Success || !Directory.Exists(TestSitePath))
	    {
        var message = "Build failed! See property BuildOutput ensure that you have a Test build configuration."
	                    + Environment.NewLine + Environment.NewLine
	                    + errorsOnlyRecorder.GetJoinedBuildMessages();
	      var buildOuput = normalRecorder.GetJoinedBuildMessages();
	      throw new BuildFailedException(message, buildOuput);
	    }
	  }

	  private BuildResult Build (IEnumerable<ILogger> loggers)
	  {
	    // 1) Clean previous deployment.
	    // 2) Do a build of the website project with the "Package" target. This will copy all
	    //    the necessary website files into a directory similar to the following:
	    //    MyProject/obj/Test/Package/PackageTmp

	    var parameters = new BuildParameters { Loggers = loggers };
	    var projectFilePath = Path.Combine(_siteRoot, _projectName + ".csproj");
	    var globalProperties = new Dictionary<string, string> { { "Configuration", "Test" } };
	    var requestData = new BuildRequestData(projectFilePath, globalProperties, null, new[] { "Clean", "Package" }, null);

	    return BuildManager.DefaultBuildManager.Build(parameters, requestData);
	  }
	}
}