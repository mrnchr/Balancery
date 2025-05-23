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

    public bool WaitMadeTurn;

    public sealed override void OnEpisodeBegin()
    {
      if (WasFirstEpisodeStarted)
      {
        WaitMadeTurn = false;
        OnEpisodeEnded();
        Environment.EndEpisode(this);
      }
      else
        WasFirstEpisodeStarted = true;

      OnEpisodeStarted();
    }

    public sealed override void OnActionReceived(ActionBuffers actions)
    {
      Environment.Academy.RecordActions(this, actions);

      if (BAcademy.IsRepetition)
        Environment.Academy.ActionProvider.InsertActions(Environment.SessionIndex, Environment.TurnIndex, ref actions);
      
      WaitMadeTurn = true;
      OnActionExecuted(actions);

      if (WaitMadeTurn)
        Environment.MakeTurn(this);
    }

    public virtual void OnActionExecuted(ActionBuffers actions)
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