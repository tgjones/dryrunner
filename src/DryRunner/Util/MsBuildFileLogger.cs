using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DryRunner.Util
{
  /// <summary>
  /// Represents a MSBuild file logger.
  /// </summary>
  [PublicAPI]
  public sealed class MsBuildFileLogger
  {
    /// <summary>
    ///  The path to the resulting log file.
    /// </summary>
    public string Path { get; private set; }

    /// <summary>
    /// The logging verbosity.
    /// </summary>
    public MsBuildVerbosity Verbosity { get; private set; }

    /// <summary>
    /// True to show the error and warning summary at the end.
    /// Defaults to <see langword="null" />.
    /// </summary>
    public bool? ShowSummary { get; set; }

    /// <summary>
    /// True to show the time that’s spent in tasks, targets, and projects.
    /// Defaults to false.
    /// </summary>
    public bool ShowPerformanceSummary { get; set; }

    /// <summary>
    /// Control which messages are shown.
    /// Defaults to <see cref="MsBuildFileLoggerMessageFilter.Everything"/>.
    /// </summary>
    public MsBuildFileLoggerMessageFilter MessageFilter { get; set; }

    /// <summary>
    /// True to suppress the list of items and properties that would appear at the start of each project build if 
    /// the verbosity level is set to <see cref="MsBuildVerbosity.Diagnostic"/>.
    /// Defaults to false.
    /// </summary>
    public bool NoItemAndPropertyList { get; set; }

    /// <summary>
    /// True to show the timestamp as a prefix to any message.
    /// Defaults to false.
    /// </summary>
    public bool ShowTimestamp { get; set; }

    /// <summary>
    /// True to show TaskCommandLineEvent messages.
    /// Defaults to false.
    /// </summary>
    public bool ShowCommandLine { get; set; }

    /// <summary>
    /// True to show the event ID for each started event, finished event, and message.
    /// Defaults to false.
    /// </summary>
    public bool ShowEventId { get; set; }

    /// <summary>
    /// True to don't align the text to the size of the console buffer.
    /// Defaults to false.
    /// </summary>
    public bool ForceNoAlign { get; set; }

    /// <summary>
    /// True to disable console colors.
    /// Defaults to false.
    /// </summary>
    public bool DisableConsoleColor { get; set; }

    /// <summary>
    /// True to enable the multiprocessor logging style even when running in non-multiprocessor mode.
    /// Defaults to <see langword="null" />.
    /// </summary>
    public bool? EnableMultiprocessorLogging { get; set; }

    /// <summary>
    /// Provides a way to provide additional flags that are not covered by the other options.
    /// </summary>
    public IEnumerable<string> AdditionalFlags { get; set; }

    /// <summary>
    /// Creates a <see cref="MsBuildFileLogger"/> with the given <paramref name="path"/> and <paramref name="verbosity"/>.
    /// </summary>
    public MsBuildFileLogger(string path, MsBuildVerbosity verbosity)
    {
      Path = path;
      Verbosity = verbosity;
      EnableMultiprocessorLogging = true;
    }

    internal string GetLoggerString(int loggerNumber)
    {
      return string.Format("/fl{0} \"/flp{0}:{1}\"", loggerNumber, string.Join(";", GetFlags()));
    }

    private IEnumerable<string> GetFlags()
    {
      yield return string.Format("LogFile={0}", Path);
      yield return string.Format("Verbosity={0}", Verbosity);

      if (ShowSummary.HasValue)
      {
        if (ShowSummary.Value)
          yield return "Summary";
        else
          yield return "NoSummary";
      }

      if (ShowPerformanceSummary)
        yield return "PerformanceSummary";

      switch (MessageFilter)
      {
        case MsBuildFileLoggerMessageFilter.Everything:
          break;
        case MsBuildFileLoggerMessageFilter.ErrorsOnly:
          yield return "ErrorsOnly";
          break;
        case MsBuildFileLoggerMessageFilter.WarningsOnly:
          yield return "WarningsOnly";
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      if (NoItemAndPropertyList)
        yield return "NoItemAndPropertyList";

      if (ShowCommandLine)
        yield return "ShowCommandLine";

      if (ShowTimestamp)
        yield return "ShowTimestamp";

      if (ShowEventId)
        yield return "ShowEventId";

      if (ForceNoAlign)
        yield return "ForceNoAlign";

      if (DisableConsoleColor)
        yield return "DisableConsoleColor";

      if (EnableMultiprocessorLogging.HasValue)
      {
        if (EnableMultiprocessorLogging.Value)
          yield return "EnableMPLogging";
        else
          yield return "DisableMPLogging";
      }

      if (AdditionalFlags != null)
        foreach (var flag in AdditionalFlags)
          yield return flag;
    }
  }
}