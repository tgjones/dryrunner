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
      var messages = _buildEvents.Select(x => x.Message);
      return string.Join(Environment.NewLine, messages);
    }
  }
}