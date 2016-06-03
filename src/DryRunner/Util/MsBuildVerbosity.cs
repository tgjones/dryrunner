using JetBrains.Annotations;

namespace DryRunner.Util
{
    /// <summary>
    /// Verbosity used for MSBuild output.
    /// </summary>
    [PublicAPI]
    public enum MsBuildVerbosity
    {
        /// <summary>
        /// Normal verbosity, which displays errors, warnings, messages with high importance, some status events, and a build summary.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Detailed verbosity, which displays errors, warnings, messages with normal or high importance, all status events, and a build summary.
        /// </summary>
        Detailed,
        /// <summary>
        /// Diagnostic verbosity, which displays all errors, warnings, messages, status events, and a build summary.
        /// </summary>
        Diagnostic,
        /// <summary>
        /// Minimal verbosity, which displays errors, warnings, messages with high importance, and a build summary.
        /// </summary>
        Minimal,
        /// <summary>
        /// Quiet verbosity, which displays a build summary.
        /// </summary>
        Quiet
    }
}