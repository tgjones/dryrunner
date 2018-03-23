using System;
using JetBrains.Annotations;

namespace DryRunner.Util
{
    /// <summary>
    /// Represents a MSBuild tools version.
    /// </summary>
    [PublicAPI]
    public class MsBuildToolsVersion
    {
        internal string Version { get; private set; }

        /// <summary>
        /// MSBuild version included in .NET Framework 2.0
        /// </summary>
        // ReSharper disable InconsistentNaming
        public static MsBuildToolsVersion v2_0 = new MsBuildToolsVersion ("2.0");

        /// <summary>
        /// MSBuild version included in .NET Framework 3.5
        /// </summary>
        public static MsBuildToolsVersion v3_5 = new MsBuildToolsVersion("3.5");

        /// <summary>
        /// MSBuild version included in .NET Framework 4.0
        /// </summary>
        public static MsBuildToolsVersion v4_0 = new MsBuildToolsVersion("4.0");

        /// <summary>
        /// MSBuild version included with Visual Studio 2013
        /// </summary>
        public static MsBuildToolsVersion v12_0 = new MsBuildToolsVersion("12.0");

        /// <summary>
        /// MSBuild version included with Visual Studio 2015
        /// </summary>
        public static MsBuildToolsVersion v14_0 = new MsBuildToolsVersion("14.0");

        /// <summary>
        /// MSBuild version included with Visual Studio 2017
        /// </summary>
        public static MsBuildToolsVersion v15_0 = new MsBuildToolsVersion("15.0");
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Enables creation of a <see cref="MsBuildToolsVersion"/> that is covered in the default cases.
        /// </summary>
        /// <param name="version">Version string (e.g. '13.5')</param>
        public static MsBuildToolsVersion Custom (string version)
        {
            return new MsBuildToolsVersion (version);
        }

        private MsBuildToolsVersion (string version)
        {
            if (string.IsNullOrWhiteSpace (version))
                throw new ArgumentException ("Version cannot be null or empty.", "version");

            Version = version;
        }

        /// <inheritdoc />
        public override string ToString ()
        {
            return Version;
        }
    }
}