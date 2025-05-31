using System;
using Mrnchr.Balancery.Runtime.Repetition;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Mrnchr.Balancery.Runtime.Academy
{
  public class BAgent : Agent
  {
    [NonSerialized]
    public bool WasFirstEpisodeStarted;

    public bool WaitMadeTurn;

    private Coroutine _routine;

    public BEnvironment Environment { get; set; }

    protected override void Awake()
    {
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
      Environment.Academy.RecordActions(this, actions);

      if (RepetitionPlayer.IsRepetition)
        Environment.Academy.ActionProvider.InsertActions(RepetitionPlayer.SessionIndex, Environment.TurnIndex,
          ref actions);

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

    public virtual void OnEpisodeFinished()
    {
    }
  }
}