using System;
using System.Runtime.Serialization;

namespace DryRunner.Exceptions
{
    [Serializable]
    public class OptionCannotBeNullOrEmptyException : OptionValidationException
    {
        public OptionCannotBeNullOrEmptyException (string optionsName, string optionProperty)
                : base("Cannot be null or empty.", optionsName, optionProperty)
        {
        }

        protected OptionCannotBeNullOrEmptyException(SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
        }
    }
}