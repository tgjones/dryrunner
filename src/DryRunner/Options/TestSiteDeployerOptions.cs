using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DryRunner.Exceptions;
using DryRunner.Util;
using Microsoft.Win32;

namespace DryRunner.Options
{
    public class TestSiteDeployerOptions
    {
        public delegate string ResolveMsBuildExePath(MsBuildToolsVersion toolsVersion, bool use32Bit);

        private readonly string[] _defaultBuildTargets = { "Clean", "Package" };

        /// <summary>
        /// Filename of the website project you want to test, including the extension (i.e. .csproj, .vbproj).
        /// This is optional - if not set, it will default to {ProjectName}.csproj.
        /// </summary>
        public string ProjectFileName { get; set; }

        /// <summary>
        /// The path to the solution file.
        /// This is used to set the SolutionDir property so that it can be used in MSBuild macros.
        /// </summary>
        public string SolutionDir { get; set; }

        /// <summary>
        /// The path to the project.  
        /// This is used to set the ProjectDir property so that it can be used in MSBuild macros.
        /// </summary>
        public string ProjectDir { get; set; }

        internal bool ProjectDirSetByUser { get; set; }

        /// <summary>
        /// The build targets that are invoked in MSBuild.
        /// Defaults to 'Clean' and 'Package'.
        /// </summary>
        public string[] BuildTargets { get; set; }

        /// <summary>
        /// Build configuration to use.
        /// Defaults to 'Test'.
        /// </summary>
        public string BuildConfiguration { get; set; }

        /// <summary>
        /// Configuration to use for the configuration file transformation (e.g. Test means that Web.Test.config is used).
        /// Defaults to <see cref="BuildConfiguration"/>.
        /// </summary>
        public string TransformationConfiguration { get; set; }

        /// <summary>
        /// Can be used to specify additional build properties that are passed to MSBuild.
        /// </summary>
        public Dictionary<string, string> AdditionalBuildProperties { get; set; }

        /// <summary>
        /// The MSBuild tools version that is used to build.
        /// Defaults to '4.0'.
        /// </summary>
        public MsBuildToolsVersion MsBuildToolsVersion { get; set; }

        /// <summary>
        /// True to use a 64-bit version of MSBuild.
        /// Defaults to false.
        /// </summary>
        public bool Use64BitMsBuild { get; set; }

        /// <summary>
        /// Function that resolves the MSBuild exe path dependent on the <see cref="MsBuildToolsVersion"/> and <see cref="Use64BitMsBuild"/>.
        /// Can be overwritten to use a non-default MSBuild exe path.
        /// </summary>
        public ResolveMsBuildExePath MsBuildExePathResolver { get; set; }

        /// <summary>
        /// True to make the IIS Express command-line window visible, otherwise false.
        /// Defaults to false.
        /// </summary>
        public bool ShowMsBuildWindow { get; set; }

        public TestSiteDeployerOptions ()
        {
            BuildConfiguration = "Test";
            BuildTargets = _defaultBuildTargets;
            MsBuildToolsVersion = MsBuildToolsVersion.v4_0;
            MsBuildExePathResolver = GetMsBuildPathFromRegistry;
        }

        internal void Validate(string optionsName)
        {
            if (string.IsNullOrWhiteSpace (BuildConfiguration))
                throw new OptionValidationException ("Configuration cannot be null or empty.", optionsName, "Configuration");

            if (!Directory.Exists (ProjectDir))
                throw new OptionValidationException (
                        string.Format ("The project directory '{0}' could not be found.", ProjectDir),
                        optionsName,
                        "ProjectDir");

            if (MsBuildExePathResolver == null)
                throw new OptionCannotBeNullException(optionsName, "MsBuildExePathResolver");

            if (BuildTargets == null)
                throw new OptionCannotBeNullException (optionsName, "BuildTargets");

            if (BuildTargets.Any (string.IsNullOrWhiteSpace))
                throw new OptionValidationException ("Cannot contain build targets that are null or empty.", optionsName, "BuildTargets");

            if (string.IsNullOrWhiteSpace(BuildConfiguration))
                throw new OptionCannotBeNullOrEmptyException(optionsName, "BuildConfiguration");

            if(MsBuildToolsVersion == null)
                throw new OptionCannotBeNullException(optionsName, "MsBuildToolsVersion");
        }

        internal void ApplyDefaultsWhereNecessary (string projectName)
        {
            if (string.IsNullOrWhiteSpace(ProjectDir))
                ProjectDir = GetPathRelativeToCurrentAssemblyPath(@"..\..\..\" + projectName);
            else
                ProjectDirSetByUser = true;

            if (string.IsNullOrWhiteSpace(ProjectFileName))
                ProjectFileName = projectName + ".csproj";
        }

        private static string GetPathRelativeToCurrentAssemblyPath(string relativePath)
        {
            var asmFilePath = new Uri(typeof(TestSiteDeployer).Assembly.CodeBase).LocalPath;
            var asmPath = Path.GetDirectoryName(asmFilePath);
            var fullPath = Path.Combine(asmPath, relativePath);
            return Path.GetFullPath(fullPath);
        }

        private static string GetMsBuildPathFromRegistry(MsBuildToolsVersion toolsVersion, bool use64Bit)
        {
            const string valueName32Bit = "MSBuildToolsPath32";
            const string valueName64Bit = "MSBuildToolsPath";

            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSBuild\ToolsVersions\" + toolsVersion.Version))
            {
                if (key == null)
                    throw new ArgumentException(
                            string.Format("For the given tools version '{0}' no registry key could be found.", toolsVersion),
                            "toolsVersion");

                var valueName = use64Bit ? valueName64Bit : valueName32Bit;
                var value = (string)key.GetValue(valueName, null);

                while (value != null && value.StartsWith("$(Registry:") && value.EndsWith(")"))
                {
                    var path = value.Substring(11, value.Length - 12).Split('@');
                    value = (string)Registry.GetValue(path[0], path[1], null);
                }

                if (use64Bit && value == null)
                    value = ((string)key.GetValue(valueName64Bit, null)).Replace("64", "");

                return Path.Combine(value, "MSBuild.exe");
            }
        }
    }
}