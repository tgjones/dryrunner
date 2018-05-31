using System;
using System.Runtime.Serialization;
using DryRunner.Options;

namespace DryRunner.Exceptions
{
  /// <summary>
  /// OptionValidationException is thrown when an invalid option value was supplied in <see cref="TestSiteDeployerOptions"/> 
  /// or <see cref="TestSiteServerOptions"/>.
  /// </summary>
  [Serializable]
  public class OptionValidationException : Exception
  {
    /// <summary>
    /// Constructs a new OptionValidationException.
    /// </summary>
    /// <param name="message">A message describing in what way the option value was invalid.</param>
    /// <param name="optionsName">Name of the options element that contained the invalid value.</param>
    /// <param name="optionProperty">Name of the option value that was invalid.</param>
    public OptionValidationException(string message, string optionsName, string optionProperty)
        : base(FormatMessage(message, optionsName, optionProperty))
    {
    }

    private static string FormatMessage(string message, string optionsName, string optionProperty)
    {
      return string.Format("Option error in '{0}.{1}': {2}", optionsName, optionProperty, message);
    }

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    protected OptionValidationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
  }
}