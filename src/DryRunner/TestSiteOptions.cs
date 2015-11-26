namespace DryRunner
{
    public class TestSiteOptions
    {
        private string[] _targets = new[] { "Clean", "Package" };

        /// <summary>
        /// Filename of the website project you want to test, including the extension (i.e. .csproj, .vbproj).
        /// This is optional - if not set, it will default to {ProjectName}.csproj.
        /// </summary>
        public string ProjectFileName { get; set; }

        /// <summary>
        /// The path to the solution file.
        /// This is use to set the ProjectDir property so that it can be used in MSBuild macros.
        /// </summary>
        public string SolutionDir { get; set; }

        /// <summary>
        /// The path to the project.  
        /// This is use to set the ProjectDir property so that it can be used in MSBuild macros.
        /// </summary>
        public string ProjectDir { get; set; }

        /// <summary>
        /// Supply additional targets in the MSBuild configuration
        /// </summary>
        public string[] Targets
        {
            get { return _targets; }
            set { _targets = value; }
        }

        /// <summary>
        /// Build configuration to use.
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// Port to use for the IIS Express instance. Defaults to 8888.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Website application path. Defaults to the server root <c>"/"</c>.
        /// </summary>
        public string ApplicationPath { get; set; }

        /// <summary>
        /// True to make the IIS Express command-line window visible, otherwise false.
        /// Defaults to true.
        /// </summary>
        public bool ShowIisExpressWindow { get; set; }

        /// <summary>
        /// Enables Windows Authentication for IIS Express.
        /// </summary>
        public bool EnableWindowsAuthentication { get; set; }

        public TestSiteOptions()
        {
            Port = 8888;
            ApplicationPath = "/";
            ShowIisExpressWindow = true;
            Configuration = "Test";
        }
    }
}