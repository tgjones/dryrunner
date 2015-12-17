using System;

namespace DryRunner.MsBuild
{
    public class MsBuildResult
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