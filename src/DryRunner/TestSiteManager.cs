using System;
using DryRunner.Options;

namespace DryRunner
{
    public class TestSiteManager
    {
        private readonly TestSiteDeployer _deployer;
        private readonly TestSiteServer _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSiteManager" /> class.
        /// </summary>
        /// <param name="projectName">Name of the web project. This is assumed to be
        /// both the project folder name, and also the <c>.csproj</c> name (without the .csproj extension).
        /// If the project folder name and .csproj name are different, 
        /// set the .csproj name in <paramref name="options"/>.</param>
        /// <param name="options">Optional configuration settings.</param>
        public TestSiteManager(string projectName, TestSiteOptions options = null)
        {
            options = options ?? new TestSiteOptions();
            options.ApplyDefaultsWhereNecessary (projectName);
            options.Validate();

            _deployer = new TestSiteDeployer(options.Deployer);
            _server = new TestSiteServer (_deployer.TestSitePath, options.Server);
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
    }
}