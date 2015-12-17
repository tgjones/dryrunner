using System;

namespace DryRunner.Options
{
    public class TestSiteOptions
    {
        public TestSiteDeployerOptions Deployer { get; set; }
        public TestSiteServerOptions Server { get; set; }

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