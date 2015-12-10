using System;
using System.Runtime.Serialization;

namespace DryRunner
{
    [Serializable]
    public class BuildFailedException : Exception
    {
        public string BuildOutput
        {
            get { return (string)Data["BuildOutput"]; }
            private set { Data.Add("BuildOutput", value); }
        }

        public BuildFailedException(string message, string buildOuput)
            : base(message)
        {
            BuildOutput = buildOuput;
        }

        public BuildFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected BuildFailedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}