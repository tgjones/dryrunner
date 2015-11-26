using System;
using System.IO;

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

            if (options.ApplicationPath == null || !options.ApplicationPath.StartsWith("/"))
                throw new ArgumentException("Application path must start with '/'.", "options");

            if (string.IsNullOrWhiteSpace (options.Configuration))
                throw new ArgumentException ("Build configuration cannot be null or empty.", "options");

            string siteRoot = (!string.IsNullOrWhiteSpace(options.ProjectDir)) ? options.ProjectDir : GetPathRelativeToCurrentAssemblyPath(@"..\..\..\" + projectName);
            if (!Directory.Exists(siteRoot))
                throw new Exception("A project with name '" + projectName + "' could not be found.");

            _deployer = new TestSiteDeployer (
                    siteRoot,
                    options.ProjectFileName ?? projectName + ".csproj",
                    options.SolutionDir,
                    options.ProjectDir,
                    options.Targets,
                    options.Configuration);

            _server = new TestSiteServer(_deployer.TestSitePath,
                options.Port, options.ApplicationPath,
                options.ShowIisExpressWindow, options.EnableWindowsAuthentication);
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

        private static string GetPathRelativeToCurrentAssemblyPath(string relativePath)
        {
            string asmFilePath = new Uri(typeof(TestSiteDeployer).Assembly.CodeBase).LocalPath;
            string asmPath = Path.GetDirectoryName(asmFilePath);
            string fullPath = Path.Combine(asmPath, relativePath);
            return Path.GetFullPath(fullPath);
        }
    }
}