using System;
using System.Runtime.Serialization;

namespace DryRunner.Exceptions
{
    /// <summary>
    /// OptionValidationException is thrown when <code>null</code> or an empty string was supplied for an option value that should not be null or empty.
    /// </summary>
    [Serializable]
    public class OptionCannotBeNullOrEmptyException : OptionValidationException
    {
        /// <summary>
        /// Constructs a new OptionCannotBeNullOrEmptyException.
        /// </summary>
        /// <param name="optionsName">Name of the options element that contained the invalid value.</param>
        /// <param name="optionProperty">Name of the option value that was invalid.</param>
        public OptionCannotBeNullOrEmptyException (string optionsName, string optionProperty)
                : base("Cannot be null or empty.", optionsName, optionProperty)
        {
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        protected OptionCannotBeNullOrEmptyException(SerializationInfo info, StreamingContext context)
                : base(info, context)
        {
        }
    }
}