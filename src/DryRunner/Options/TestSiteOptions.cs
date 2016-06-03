using JetBrains.Annotations;

namespace DryRunner.Options
{
    /// <summary>
    /// Options for customizing the test site deployment and hosting.
    /// </summary>
    [PublicAPI]
    public class TestSiteOptions
    {
        /// <summary>
        /// Options for customizing the test site deployment.
        /// </summary>
        public TestSiteDeployerOptions Deployer { get; set; }

        /// <summary>
        /// Options for customizing the test site hosting.
        /// </summary>
        public TestSiteServerOptions Server { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TestSiteOptions()
        {
            Deployer = new TestSiteDeployerOptions();
            Server = new TestSiteServerOptions();
        }

        internal void Validate()
        {
            Deployer.Validate("Deployer");
            Server.Validate("Server");
        }

        internal void ApplyDefaultsWhereNecessary(string projectName)
        {
            if (Deployer == null)
                Deployer = new TestSiteDeployerOptions();

            if (Server == null)
                Server = new TestSiteServerOptions();

            Deployer.ApplyDefaultsWhereNecessary(projectName);
        }
    }
}