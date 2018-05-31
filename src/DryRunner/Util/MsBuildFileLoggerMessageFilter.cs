namespace DryRunner.Util
{
  /// <summary>
  /// Allows filtering the messages shown by a <see cref="MsBuildFileLogger"/>.
  /// </summary>
  public enum MsBuildFileLoggerMessageFilter
  {
    /// <summary>
    /// Show all messages.
    /// </summary>
    Everything,

    /// <summary>
    /// Show only error messages.
    /// </summary>
    ErrorsOnly,

    /// <summary>
    /// Show only warning messages.
    /// </summary>
    WarningsOnly
  }
}