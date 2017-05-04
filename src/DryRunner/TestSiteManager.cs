using DryRunner.Options;

namespace DryRunner
{
    /// <summary>
    /// Deploys and hosts a test site (by using <see cref="TestSiteDeployer"/> and <see cref="TestSiteServer"/>.
    /// </summary>
    public class TestSiteManager
    {
        private readonly TestSiteDeployer _deployer;
        private readonly TestSiteServer _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSiteManager" /> class.
        /// </summary>
        /// <param name="deployerOptions">Options for the deployment (MsBuild deploy/publish).</param>
        /// <param name="serverOptions">Options for the server (IIS Express).</param>
        public TestSiteManager(TestSiteDeployerOptions deployerOptions, TestSiteServerOptions serverOptions = null)
        {
            _deployer = new TestSiteDeployer(deployerOptions);
            _server = new TestSiteServer(_deployer, serverOptions ?? new TestSiteServerOptions());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSiteManager" /> class.
        /// </summary>
        /// <param name="projectName">Name of the web project. This is assumed to be
        /// both the project folder name, and also the <c>.csproj</c> name (without the .csproj extension).
        /// If the project folder name and .csproj name are different, use the 
        /// <see cref="TestSiteManager(TestSiteDeployerOptions,TestSiteServerOptions)" /> constructor.
        /// </param>
        /// <param name="serverOptions">Options for the server (IIS Express).</param>
        public TestSiteManager(string projectName, TestSiteServerOptions serverOptions = null) 
            : this(new TestSiteDeployerOptions(projectName), serverOptions)
        {
        }

        /// <summary>
        /// Deploys and starts the configured test site.
        /// </summary>
        public void Start()
        {
            _deployer.Deploy();
            _server.Start();
        }

        /// <summary>
        /// Stops the configured test site.
        /// </summary>
        public void Stop()
        {
            _server.Stop();
        }
    }
}