using System;
using System.Runtime.Serialization;

namespace DryRunner.Exceptions
{
    [Serializable]
    public class OptionCannotBeNullException : OptionValidationException
    {
        public OptionCannotBeNullException (string optionsName, string optionProperty)
                : base("Cannot be null.", optionsName, optionProperty)
        {
        }

        protected OptionCannotBeNullException(SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
        }
    }
}