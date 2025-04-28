using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

namespace Mrnchr.Balancery.Runtime
{
  public class BAgent : Agent
  {
    public BEnvironment Environment { get; set; }
    
    [NonSerialized]
    public bool WasFirstEpisodeStarted;

    public sealed override void OnEpisodeBegin()
    {
      if (WasFirstEpisodeStarted)
      {
        OnEpisodeEnded();
        Environment.EndEpisode(this);
      }
      else
        WasFirstEpisodeStarted = true;

      OnEpisodeStarted();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
      SetReward(0);
      EndEpisode();
    }

    public virtual void OnEpisodeStarted()
    {
    }

    public virtual void OnEpisodeEnded()
    {
    }
  }
}