using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DryRunner.Options;
using Microsoft.Win32;

namespace DryRunner.MsBuild
{
    public static class MsBuildUtility
    {
        public static string GetMsBuildPathFromRegistry(MsBuildToolsVersion toolsVersion, bool use64Bit)
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

        public static MsBuildResult Build(TestSiteDeployerOptions options)
        {
            var projectFilePath = Path.Combine(options.ProjectDir, options.ProjectFileName);

            var properties = new Dictionary<string, string>();
            properties.Add("Configuration", options.BuildConfiguration);

            if (!string.IsNullOrWhiteSpace(options.SolutionDir))
                properties.Add("SolutionDir", options.SolutionDir);

            if (!string.IsNullOrWhiteSpace(options.ProjectDir))
                properties.Add("ProjectDir", options.ProjectDir);

            if (!string.IsNullOrWhiteSpace(options.TransformationConfiguration))
                properties.Add("ProjectConfigTransformFileName", "Web." + options.TransformationConfiguration + ".config");

            if (options.AdditionalBuildProperties != null)
                foreach (var property in options.AdditionalBuildProperties)
                    properties.Add(property.Key, property.Value);

            using (TemporaryFile normalLogFile = new TemporaryFile(), errorLogFile = new TemporaryFile())
            {
                var path = options.MsBuildExePathResolver(options.MsBuildToolsVersion, options.Use64BitMsBuild);
                var arguments = string.Format(
                        "{0} /p:{1} /t:{2} /fl1 /flp1:{3} /fl2 /flp2:{4}",
                        projectFilePath,
                        string.Join(";", properties.Select(kvp => kvp.Key + "=" + kvp.Value)),
                        string.Join(";", options.BuildTargets),
                        string.Format("LogFile={0};Verbosity=Normal", normalLogFile.Path),
                        string.Format("LogFile={0};ErrorsOnly", errorLogFile.Path)
                        );

                var process = Process.Start (new ProcessStartInfo (path, arguments) { CreateNoWindow = !options.ShowMsBuildWindow, UseShellExecute = false });
                Trace.Assert(process != null, "process != null");
                process.WaitForExit();

                return process.ExitCode == 0
                        ? MsBuildResult.Success(normalLogFile.GetContents())
                        : MsBuildResult.Failure(normalLogFile.GetContents(), errorLogFile.GetContents());
            }
        }

        private class TemporaryFile : IDisposable
        {
            public string Path { get; private set; }

            public TemporaryFile()
            {
                Path = System.IO.Path.GetTempFileName();
            }

            public string GetContents()
            {
                return File.ReadAllText(Path);
            }

            public void Dispose()
            {
                File.Delete(Path);
            }
        }
    }
}