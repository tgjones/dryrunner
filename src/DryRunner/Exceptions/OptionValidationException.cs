using System;
using System.Runtime.Serialization;

namespace DryRunner.Exceptions
{
    [Serializable]
    public class OptionValidationException : Exception
    {
        public OptionValidationException(string message, string optionsName, string optionProperty)
            : base(FormatMessage(message, optionsName, optionProperty))
        {
        }

        private static string FormatMessage(string message, string optionsName, string optionProperty)
        {
            return string.Format("Option error in '{0}.{1}': {2}", optionsName, optionProperty, message);
        }

        protected OptionValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}