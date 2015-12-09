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
        private readonly TestSiteOptions _options;

        public string TestSitePath
        {
            get { return Path.Combine(_siteRoot, @"obj\" + _options.Configuration + @"\Package\PackageTmp"); }
        }

        public TestSiteDeployer (string siteRoot, TestSiteOptions options)
        {
            _siteRoot = siteRoot;
            _options = options;
        }

        public void Deploy()
        {
            var errorsOnlyRecorder = new RecordingEventRedirector();
            var normalRecorder = new RecordingEventRedirector();
            var loggers = new ILogger[]
	        {
	            new ConsoleLogger(LoggerVerbosity.Quiet),
	            new ConfigurableForwardingLogger
	            {
	                BuildEventRedirector = errorsOnlyRecorder,
	                Verbosity = LoggerVerbosity.Quiet
	            },
	            new ConfigurableForwardingLogger
	            {
	                BuildEventRedirector = normalRecorder,
	                Verbosity = LoggerVerbosity.Normal
	            }
	        };

            var result = Build(loggers);

            if (result.OverallResult != BuildResultCode.Success || !Directory.Exists(TestSitePath))
            {
                var message = "Build failed! See property BuildOutput ensure that you have a " + _options.Configuration + " build configuration."
                      + Environment.NewLine + Environment.NewLine
                      + errorsOnlyRecorder.GetJoinedBuildMessages();
                var buildOuput = normalRecorder.GetJoinedBuildMessages();
                throw new BuildFailedException(message, buildOuput);
            }
        }

        private BuildResult Build(IEnumerable<ILogger> loggers)
        {
            // 1) Clean previous deployment.
            // 2) Do a build of the website project with the "Package" target. This will copy all
            //    the necessary website files into a directory similar to the following:
            //    MyProject/obj/{Configuration}/Package/PackageTmp

            var parameters = new BuildParameters { Loggers = loggers };
            var projectFilePath = Path.Combine(_siteRoot, _options.ProjectFileName);
            var globalProperties = new Dictionary<string, string>();

            globalProperties.Add("Configuration", string.IsNullOrEmpty(_options.Configuration) ? "Test" : _options.Configuration);

            if (!string.IsNullOrWhiteSpace(_options.SolutionDir))
                globalProperties.Add("SolutionDir", _options.SolutionDir);

            if (!string.IsNullOrWhiteSpace(_options.ProjectDir))
                globalProperties.Add("ProjectDir", _options.ProjectDir);

            if (!string.IsNullOrWhiteSpace(_options.TransformationConfiguration))
                globalProperties.Add("ProjectConfigTransformFileName", "Web." + _options.TransformationConfiguration + ".config");

            if(_options.Properties != null)
                foreach (var property in _options.Properties)
                    globalProperties.Add (property.Key, property.Value);

            var requestData = new BuildRequestData (projectFilePath, globalProperties, null, _options.Targets, null);

            return BuildManager.DefaultBuildManager.Build(parameters, requestData);
        }
    }
}