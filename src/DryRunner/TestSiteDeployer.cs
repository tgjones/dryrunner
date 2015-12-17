using System;
using System.IO;
using DryRunner.Exceptions;
using DryRunner.MsBuild;
using DryRunner.Options;

namespace DryRunner
{
    public class TestSiteDeployer
    {
        private readonly TestSiteDeployerOptions _options;

        public string TestSitePath
        {
            get { return Path.Combine(_options.ProjectDir, @"obj\" + _options.BuildConfiguration + @"\Package\PackageTmp"); }
        }

        public TestSiteDeployer(TestSiteDeployerOptions options)
        {
            _options = options;
        }

        public void Deploy()
        {
            var result = MsBuildUtility.Build(_options);

            if (!result.WasSuccessful|| !Directory.Exists(TestSitePath))
            {
                var message = "Build failed! See property BuildOutput ensure that you have a " + _options.BuildConfiguration + " build configuration."
                              + Environment.NewLine + Environment.NewLine
                              + result.ErrorOutput;
                throw new BuildFailedException(message, result.Output);
            }
        }
    }
}