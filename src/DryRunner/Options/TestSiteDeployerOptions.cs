using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DryRunner.Exceptions;
using DryRunner.Util;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Setup.Configuration;
using Microsoft.Win32;

namespace DryRunner.Options
{
  /// <summary>
  /// Options for customizing the way a test site is deployed (=built and packaged with MSBuild)
  /// </summary>
  [PublicAPI]
  public class TestSiteDeployerOptions
  {
    /// <summary>
    /// The default <see cref="MsBuildToolsVersion"/> used for all new instances of <see cref="TestSiteDeployerOptions"/>.
    /// </summary>
    public static MsBuildToolsVersion DefaultMsBuildToolsVersion { get; set; } = MsBuildToolsVersion.v4_0;

    /// <summary>
    /// Returns a path to an MSBuild executable dependent on the <paramref name="toolsVersion"/> and the <paramref name="use32Bit"/> flag.
    /// </summary>
    public delegate string ResolveMsBuildExePath(MsBuildToolsVersion toolsVersion, bool use32Bit);

    private readonly string[] _defaultBuildTargets = { "Clean", "Package" };

    /// <summary>
    /// Name of the directory containing the project file.
    /// With a full project file path of 'C:\Solution\Project\ProjectFile.csproj' the ProjectFolderName would be 'Project'.
    /// </summary>
    public string ProjectFolderName { get; private set; }

    /// <summary>
    /// File name of the website project you want to test, including the extension (e.g. .csproj, .vbproj).
    /// </summary>
    public string ProjectFileName { get; private set; }

    internal string ProjectFilePath { get; private set; }

    /// <summary>
    /// The path to the solution file.
    /// This is used to set the SolutionDir property so that it can be used in MSBuild macros.
    /// </summary>
    public string SolutionDir { get; set; }

    internal string ProjectDir { get; private set; }

    /// <summary>
    /// The directory the web application is deployed to (defaults to "{ProjectDir}\obj\{BuildConfiguration}\Package\PackageTmp").
    /// Note that, the deploy directory is not automatically cleaned up, this has to be done outside of DryRunner.
    /// </summary>
    public string DeployDirectory { get; set; }

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
    /// The verbosity used for MSBuild log output.
    /// Defaults to <see cref="F:MsBuildVerbosity.Normal"/>.
    /// </summary>
    public MsBuildVerbosity MsBuildVerbosity { get; set; }

    /// <summary>
    /// Allows specifying additional <see cref="MsBuildFileLogger"/>s to be used. 
    /// Only allows up to 7 additional <see cref="MsBuildFileLogger"/>s (since MSBuild only supports 9 and 2 are already used internally)!
    /// </summary>
    public IEnumerable<MsBuildFileLogger> AdditionalMsBuildFileLoggers { get; set; }

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

    private TestSiteDeployerOptions()
    {
      BuildConfiguration = "Test";
      BuildTargets = _defaultBuildTargets;
      MsBuildToolsVersion = DefaultMsBuildToolsVersion;
      MsBuildExePathResolver = GetMsBuildPathFromRegistry;
    }

    /// <summary>
    /// Creates new <see cref="TestSiteDeployerOptions"/>. 
    /// Use this constructor when the project folder and file name are equal (e.g. 'C:\Solution\Project\Project.csproj').
    /// </summary>
    public TestSiteDeployerOptions(string projectName)
        : this(projectName, projectName + ".csproj")
    {
    }

    /// <summary>
    /// Creates new <see cref="TestSiteDeployerOptions"/>. 
    /// Use this constructor when the project folder and file name are not equal (e.g. 'C:\Solution\ProjectFolder\ProjectFile.csproj').
    /// </summary>
    public TestSiteDeployerOptions(string projectFolderName, string projectFileName)
        : this()
    {
      if (projectFolderName == null)
        throw new ArgumentNullException("projectFolderName");

      if (projectFileName == null)
        throw new ArgumentNullException("projectFileName");

      ProjectFolderName = projectFolderName;
      ProjectFileName = projectFileName;
    }

    internal void FinalizeAndValidate()
    {
      ApplyDefaultsWhereNecessary();
      Validate();
    }

    private void Validate()
    {
      const string optionsName = "Deployer";

      if (string.IsNullOrWhiteSpace(BuildConfiguration))
        throw new OptionValidationException("Configuration cannot be null or empty.", optionsName, "Configuration");

      if (!Directory.Exists(SolutionDir))
        throw new OptionValidationException(
            string.Format("The solution directory '{0}' could not be found.", SolutionDir),
            optionsName,
            "SolutionDir");

      if (!Directory.Exists(ProjectDir))
        throw new OptionValidationException(
            string.Format("The project directory '{0}' could not be found.", ProjectDir),
            optionsName,
            "ProjectDir");

      if (!File.Exists(ProjectFilePath))
        throw new OptionValidationException(
            string.Format("The project file '{0}' could not be found.", ProjectFilePath),
            optionsName,
            "ProjectFileName");

      if (MsBuildExePathResolver == null)
        throw new OptionCannotBeNullException(optionsName, "MsBuildExePathResolver");

      if (BuildTargets == null)
        throw new OptionCannotBeNullException(optionsName, "BuildTargets");

      if (BuildTargets.Any(string.IsNullOrWhiteSpace))
        throw new OptionValidationException("Cannot contain build targets that are null or empty.", optionsName, "BuildTargets");

      if (string.IsNullOrWhiteSpace(BuildConfiguration))
        throw new OptionCannotBeNullOrEmptyException(optionsName, "BuildConfiguration");

      if (MsBuildToolsVersion == null)
        throw new OptionCannotBeNullException(optionsName, "MsBuildToolsVersion");

      if (AdditionalMsBuildFileLoggers != null && AdditionalMsBuildFileLoggers.Count() > 7)
        throw new OptionValidationException(
            "It is not possible to supply more than 7 additional file loggers " +
            "(since MSBuild only supports 9 in total and 2 are already used internally).",
            optionsName,
            "AdditionalMsBuildFileLoggers");
    }

    private void ApplyDefaultsWhereNecessary()
    {
      if (string.IsNullOrWhiteSpace(SolutionDir))
        SolutionDir = GetPathRelativeToCurrentAssemblyPath(@"..\..\..\");

      ProjectDir = Path.Combine(SolutionDir, ProjectFolderName);
      ProjectFilePath = Path.Combine(ProjectDir, ProjectFileName);

      if (string.IsNullOrEmpty(DeployDirectory))
        DeployDirectory = Path.Combine(ProjectDir, "obj", BuildConfiguration, "Package", "PackageTmp");
    }

    private static string GetPathRelativeToCurrentAssemblyPath(string relativePath)
    {
      var asmFilePath = new Uri(typeof(TestSiteDeployer).Assembly.CodeBase).LocalPath;
      var asmPath = Path.GetDirectoryName(asmFilePath);
      Debug.Assert(asmPath != null, "asmPath != null");
      var fullPath = Path.Combine(asmPath, relativePath);
      return Path.GetFullPath(fullPath);
    }

    private static string GetMsBuildPathFromRegistry(MsBuildToolsVersion toolsVersion, bool use64Bit)
    {
      if (toolsVersion == MsBuildToolsVersion.v15_0)
      {
        // VS2017 comes with MsBuild tools version 15 which does not include a registry key
        int arrSize = 10, instanceCount;
        var arr = new ISetupInstance[arrSize];
        new SetupConfiguration().EnumInstances().Next(arrSize, arr, out instanceCount);

        var instance = arr.FirstOrDefault(i => i.GetInstallationVersion().Contains("15"));

        if (instance == null)
          throw new InvalidOperationException(
              string.Format("Could not find MSBuild executable for version '{0}'.", toolsVersion));

        var msBuildSubFolder = use64Bit ? @"MSBuild\15.0\Bin\amd64" : @"MSBuild\15.0\Bin\";
        return Path.Combine(instance.GetInstallationPath(), msBuildSubFolder, "MSBuild.exe");
      }

      const string valueName32Bit = "MSBuildToolsPath32";
      const string valueName64Bit = "MSBuildToolsPath";

      using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSBuild\ToolsVersions\" + toolsVersion.Version))
      {
        if (key == null)
          throw new ArgumentException(
              string.Format("For the given tools version '{0}' no registry key could be found.", toolsVersion),
              "toolsVersion");

        var valueName = use64Bit ? valueName64Bit : valueName32Bit;
        var value = (string) key.GetValue(valueName, null);

        while (value != null && value.StartsWith("$(Registry:") && value.EndsWith(")"))
        {
          var path = value.Substring(11, value.Length - 12).Split('@');
          value = (string) Registry.GetValue(path[0], path[1], null);
        }

        if (use64Bit && value == null)
          value = ((string) key.GetValue(valueName64Bit, null)).Replace("64", "");

        Debug.Assert(value != null, "value != null");
        return Path.Combine(value, "MSBuild.exe");
      }
    }
  }
}