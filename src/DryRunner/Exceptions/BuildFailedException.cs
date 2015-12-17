using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace DryRunner.Exceptions
{
    /// <summary>
    /// BuildFailedException is thrown when the MSBuild build process failed.
    /// </summary>
    [Serializable]
    [PublicAPI]
    public class BuildFailedException : Exception
    {
        /// <summary>
        /// Contains all build output.
        /// </summary>
        public string BuildOutput
        {
            get { return (string)Data["BuildOutput"]; }
            private set { Data.Add("BuildOutput", value); }
        }

        /// <summary>
        /// Constructs a BuildFailed exception.
        /// </summary>
        public BuildFailedException(string message, string buildOuput)
            : base(message)
        {
            BuildOutput = buildOuput;
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        protected BuildFailedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}