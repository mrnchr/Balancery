using System.Collections;
using System.Collections.Generic;
using Mrnchr.Balancery.Runtime.Repetition;
using UnityEngine;

namespace Mrnchr.Balancery.Runtime.Academy
{
  public class BEnvironment : MonoBehaviour
  {
    private readonly Dictionary<BAgent, bool> _finishEpisodeFlags = new Dictionary<BAgent, bool>();
    private readonly Dictionary<BAgent, bool> _makeTurnFlags = new Dictionary<BAgent, bool>();
    private readonly Dictionary<BAgent, bool> _startEpisodeFlags = new Dictionary<BAgent, bool>();

    [SerializeField]
    private List<BAgent> _agents = new List<BAgent>();

    private Coroutine _routine;
    private bool _wasFirstEpisodeStarted;

    public BAcademy Academy { get; set; }
    public int SessionIndex { get; set; }
    public int TurnIndex { get; private set; }

    public List<BAgent> Agents => _agents;

    private void Awake()
    {
      foreach (BAgent agent in _agents)
      {
        agent.Environment = this;
      }

      InitializeFlags(_finishEpisodeFlags);
      InitializeFlags(_makeTurnFlags);
      InitializeFlags(_startEpisodeFlags);

      if (RepetitionPlayer.IsRepetition)
      {
        RepetitionPlayer.OnPause += ClearRoutine;
        RepetitionPlayer.OnResume += RestartRoutine;
        RepetitionPlayer.OnNextTurn += NextTurn;
      }
    }

    private void OnDestroy()
    {
      RepetitionPlayer.OnPause -= ClearRoutine;
      RepetitionPlayer.OnResume -= RestartRoutine;
      RepetitionPlayer.OnNextTurn -= NextTurn;
    }

    private void RestartRoutine()
    {
      ClearRoutine();

      _routine = StartCoroutine(MakeDecisionRoutine());
    }

    private void ClearRoutine()
    {
      if (_routine != null)
      {
        StopCoroutine(_routine);
        _routine = null;
      }
    }

    private IEnumerator MakeDecisionRoutine()
    {
      while (RepetitionPlayer.IsRepetition)
      {
        float speed = RepetitionPlayer.SimulationSpeed;
        yield return new WaitForSeconds(speed != 0 ? 1 / speed : 0);
        NextTurn();
      }
    }

    private void NextTurn()
    {
      _agents[TurnIndex % 2].RequestDecision();
    }

    public void MarkReadyToStart(BAgent agent)
    {
      _startEpisodeFlags[agent] = true;

      if (CheckAll(_startEpisodeFlags) && (Academy.CanContinueSimulation() || !_wasFirstEpisodeStarted))
      {
        _wasFirstEpisodeStarted = true;
        StartEpisode();
      }
    }

    private void StartEpisode()
    {
      foreach (BAgent agent in _agents)
        agent.StartEpisode();
        
      InitializeFlags(_startEpisodeFlags);
      RestartRoutine();
    }

    public void MarkReadyToFinish(BAgent agent)
    {
      _finishEpisodeFlags[agent] = true;

      if (CheckAll(_finishEpisodeFlags))
        ClearAndFinishEpisode();
    }

    private void ClearAndFinishEpisode()
    {
      ClearRoutine();
      if (!Academy.CanContinueSimulation())
        return;
        
      FinishEpisode();
    }

    public void ContinueSimulation()
    {
      FinishEpisode();
      StartEpisode();
    }

    private void FinishEpisode()
    {
      Academy.CompleteEpisode(this);

      foreach (BAgent agent in _agents)
        agent.FinishEpisode();

      TurnIndex = 0;
      SessionIndex = Academy.StartSimulationCount;
      InitializeFlags(_finishEpisodeFlags);
    }

    public void MakeTurn(BAgent agent)
    {
      _makeTurnFlags[agent] = true;
      TurnIndex++;

      if (CheckAll(_makeTurnFlags))
        InitializeFlags(_makeTurnFlags);
    }

    private void InitializeFlags(Dictionary<BAgent, bool> flags)
    {
      flags.Clear();
      foreach (BAgent agent in _agents)
      {
        flags.Add(agent, false);
      }
    }

    private bool CheckAll(Dictionary<BAgent, bool> flags)
    {
      foreach (KeyValuePair<BAgent, bool> flag in flags)
      {
        if (!flag.Value)
          return false;
      }

      return true;
    }
  }
}