using System;
using System.Runtime.Serialization;

namespace DryRunner.Exceptions
{
  /// <summary>
  /// OptionValidationException is thrown when <code>null</code> was supplied for an option value that should not be null.
  /// </summary>
  [Serializable]
  public class OptionCannotBeNullException : OptionValidationException
  {
    /// <summary>
    /// Constructs a new OptionCannotBeNullException.
    /// </summary>
    /// <param name="optionsName">Name of the options element that contained the invalid value.</param>
    /// <param name="optionProperty">Name of the option value that was invalid.</param>
    public OptionCannotBeNullException(string optionsName, string optionProperty)
        : base("Cannot be null.", optionsName, optionProperty)
    {
    }

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    protected OptionCannotBeNullException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
  }
}