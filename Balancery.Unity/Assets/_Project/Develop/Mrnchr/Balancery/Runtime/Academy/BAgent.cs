using System;
using Mrnchr.Balancery.Runtime.Repetition;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
#if UNITY_EDITOR
using Unity.Profiling;
#endif

namespace Mrnchr.Balancery.Runtime.Academy
{
  public class BAgent : Agent
  {
#if UNITY_EDITOR
    private static ProfilerMarker _recordActionProfiler = new ProfilerMarker("RecordActions");
    private static ProfilerMarker _markMadeTurnProfiler = new ProfilerMarker("MarkMadeTurn");
    private static ProfilerMarker _onActionExecutedProfiler = new ProfilerMarker("OnActionExecuted");
#endif

    [NonSerialized]
    public bool WasFirstEpisodeStarted;

    [NonSerialized]
    public bool WaitMadeTurn;

    public BEnvironment Environment { get; set; }

    protected override void Awake()
    {
      Environment = GetComponentInParent<BEnvironment>();
      if (RepetitionPlayer.IsRepetition)
      {
        if (TryGetComponent(out DecisionRequester requester))
          Destroy(requester);
      }

      base.Awake();
    }

    public sealed override void OnEpisodeBegin()
    {
      if (WasFirstEpisodeStarted)
        Environment.MarkReadyToFinish(this);
      else
        WasFirstEpisodeStarted = true;

      Environment.MarkReadyToStart(this);
    }

    public void StartEpisode()
    {
      OnEpisodeStarted();
    }

    public void FinishEpisode()
    {
      WaitMadeTurn = false;
      OnEpisodeFinished();
    }

    public sealed override void OnActionReceived(ActionBuffers actions)
    {
      if (RepetitionPlayer.IsRepetition)
      {
        Environment.Academy.ActionProvider.InsertActions(RepetitionPlayer.SessionIndex, Environment.TurnIndex,
          ref actions);
      }
      else
      {
#if UNITY_EDITOR
        using (_recordActionProfiler.Auto())
#endif
        {
          Environment.Academy.RecordActions(this, actions);
        }
      }

      WaitMadeTurn = true;
#if UNITY_EDITOR
      using (_onActionExecutedProfiler.Auto())
#endif
      {
        OnActionExecuted(actions);
      }

      if (WaitMadeTurn)
      {
#if UNITY_EDITOR
        using (_markMadeTurnProfiler.Auto())
#endif
        {
          Environment.MarkMadeTurn(this);
        }
      }
    }

    public virtual void OnActionExecuted(ActionBuffers actions)
    {
      SetReward(0);
      EndEpisode();
    }

    public virtual void OnEpisodeStarted()
    {
    }

    public virtual void OnEpisodeFinished()
    {
    }
  }
}