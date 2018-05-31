using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DryRunner.Exceptions;
using DryRunner.Options;
using DryRunner.Util;

namespace DryRunner
{
  /// <summary>
  /// Builds and packages the test site using MSBuild.
  /// </summary>
  public class TestSiteDeployer
  {
    private readonly TestSiteDeployerOptions _options;

    /// <summary>
    /// The path at which the built and packaged test site lies.
    /// </summary>
    public string TestSitePath
    {
      get { return _options.DeployDirectory; }
    }

    /// <summary>
    /// Creates a new test site deployed using options defined in <paramref name="options"/>.
    /// </summary>
    public TestSiteDeployer(TestSiteDeployerOptions options)
    {
      options.FinalizeAndValidate();
      _options = options;
    }

    /// <summary>
    /// Deploys (builds and packages) the test site.
    /// </summary>
    public void Deploy()
    {
      var result = Build(_options);

      if (!result.WasSuccessful || !Directory.Exists(TestSitePath))
      {
        var message = "Build failed! See property BuildOutput ensure that you have a " + _options.BuildConfiguration + " build configuration."
                      + Environment.NewLine + Environment.NewLine
                      + result.ErrorOutput;
        throw new BuildFailedException(message, result.Output);
      }
    }

    /// <summary>
    /// Allows easily changing the configuration file (Web.config).
    /// </summary>
    public void ChangeConfiguration(Func<XDocument, XDocument> changeConfiguration)
    {
      var configFilePath = Path.Combine(TestSitePath, "Web.config");

      if(!File.Exists(configFilePath))
        throw new InvalidOperationException($"Could not find config file at '{configFilePath}'.");

      var xDocument = XDocument.Load(configFilePath);
      xDocument = changeConfiguration(xDocument);
      xDocument.Save(configFilePath);
    }

    private MsBuildResult Build(TestSiteDeployerOptions options)
    {
      var properties = new Dictionary<string, string>
                       {
                           { "Configuration", options.BuildConfiguration },
                           { "SolutionDir", options.SolutionDir },
                           { "_PackageTempDir", options.DeployDirectory }
                       };

      if (!string.IsNullOrWhiteSpace(options.TransformationConfiguration))
        properties.Add("ProjectConfigTransformFileName", "Web." + options.TransformationConfiguration + ".config");

      if (options.AdditionalBuildProperties != null)
        foreach (var property in options.AdditionalBuildProperties)
          properties.Add(property.Key, property.Value);

      using (TemporaryFile defaultLogFile = new TemporaryFile(), errorLogFile = new TemporaryFile())
      {
        var path = options.MsBuildExePathResolver(options.MsBuildToolsVersion, options.Use64BitMsBuild);

        var additionalLoggers = options.AdditionalMsBuildFileLoggers != null &&
                                options.AdditionalMsBuildFileLoggers.Any()
            ? string.Join(" ", options.AdditionalMsBuildFileLoggers.Select((l, i) => l.GetLoggerString(i + 3)))
            : "";

        var arguments = string.Format(
                @"""{0}"" ""/p:{1}"" ""/t:{2}"" ""/v:{3}"" /fl1 ""/flp1:{4}"" /fl2 ""/flp2:{5}"" {6}",
                options.ProjectFilePath,
                string.Join(";", properties.Select(kvp => kvp.Key + "=" + kvp.Value)),
                string.Join(";", options.BuildTargets),
                options.MsBuildVerbosity,
                string.Format("LogFile={0};Verbosity={1}", defaultLogFile.Path, options.MsBuildVerbosity),
                string.Format("LogFile={0};ErrorsOnly", errorLogFile.Path),
                additionalLoggers
            )
            // Fix escaping, since \" would lead to errors (since it would escape the quotes). 
            // This happens mostly with directory properties (e.g. SolutionDir) which should end with a \.
            .Replace(@"\""", @"\\""");

        var process = Process.Start(new ProcessStartInfo(path, arguments) { CreateNoWindow = !options.ShowMsBuildWindow, UseShellExecute = false });
        Trace.Assert(process != null, "process != null");
        process.WaitForExit();

        return process.ExitCode == 0
            ? MsBuildResult.Success(defaultLogFile.GetContents())
            : MsBuildResult.Failure(defaultLogFile.GetContents(), errorLogFile.GetContents());
      }
    }

    private class MsBuildResult
    {
      public bool WasSuccessful { get; private set; }
      public string ErrorOutput { get; private set; }
      public string Output { get; private set; }

      private MsBuildResult(bool wasSuccessful, string output, string errorOutput)
      {
        WasSuccessful = wasSuccessful;
        ErrorOutput = errorOutput;
        Output = output;
      }

      public static MsBuildResult Success(string output)
      {
        return new MsBuildResult(true, output, null);
      }

      public static MsBuildResult Failure(string output, string errorOutput)
      {
        return new MsBuildResult(false, output, errorOutput);
      }
    }
  }
}