using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using System.Linq;

namespace DryRunner
{
  public class RecordingEventRedirector : IEventRedirector
  {
    private readonly List<BuildEventArgs> _buildEvents = new List<BuildEventArgs>();

    public IEnumerable<BuildEventArgs> BuildEvents
    {
      get { return _buildEvents; }
    }

    public void ForwardEvent (BuildEventArgs buildEvent)
    {
      _buildEvents.Add(buildEvent);
    }

    public string GetJoinedBuildMessages ()
    {
      var messages = _buildEvents.Select(BuildMessage);
      return string.Join(Environment.NewLine, messages);
    }

    private object BuildMessage (BuildEventArgs buildEvent)
    {
      var error = buildEvent as BuildErrorEventArgs;
      if (error == null)
        return string.Format("{0}: {1}", buildEvent.SenderName, buildEvent.Message);

      return string.Format("{0}({1}/{2}): error {3}: {4}", error.File, error.LineNumber, error.ColumnNumber, error.Code, error.Message);
    }
  }
}