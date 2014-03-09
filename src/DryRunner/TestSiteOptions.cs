namespace DryRunner
{
    public class TestSiteOptions
    {
        /// <summary>
        /// Filename of the website project you want to test, including the extension (i.e. .csproj, .vbproj).
        /// This is optional - if not set, it will default to {ProjectName}.csproj.
        /// </summary>
        public string ProjectFileName { get; set; }

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

        public TestSiteOptions()
        {
            Port = 8888;
            ApplicationPath = "/";
            ShowIisExpressWindow = true;
        }
    }
}