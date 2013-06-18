using System;
using System.Runtime.Serialization;

namespace DryRunner
{
  [Serializable]
  public class BuildFailedException : Exception
  {
    public BuildFailedException (string message)
        : base(message)
    {
    }

    public BuildFailedException (string message, Exception inner)
        : base(message, inner)
    {
    }

    protected BuildFailedException (
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }
  }
}