using System;

namespace DryRunner.MsBuild
{
    public class MsBuildToolsVersion
    {
        public string Version { get; private set; }

        public static MsBuildToolsVersion v2_0 = new MsBuildToolsVersion ("2.0");
        public static MsBuildToolsVersion v3_5 = new MsBuildToolsVersion("3.5");
        public static MsBuildToolsVersion v4_0 = new MsBuildToolsVersion("4.0");
        public static MsBuildToolsVersion v12_0 = new MsBuildToolsVersion("12.0");
        public static MsBuildToolsVersion v14_0 = new MsBuildToolsVersion("14.0");

        public static MsBuildToolsVersion Custom (string version)
        {
            return new MsBuildToolsVersion (version);
        }

        private MsBuildToolsVersion (string version)
        {
            if (string.IsNullOrWhiteSpace (version))
                throw new ArgumentException ("Cannot be null or empty.", "version");

            Version = version;
        }

        public override string ToString ()
        {
            return Version;
        }
    }
}