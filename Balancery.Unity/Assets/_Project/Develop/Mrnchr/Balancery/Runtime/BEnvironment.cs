using System.Collections.Generic;
using UnityEngine;

namespace Mrnchr.Balancery.Runtime
{
  public class BEnvironment : MonoBehaviour
  {
    private readonly Dictionary<BAgent, bool> _endEpisodeFlags = new Dictionary<BAgent, bool>();
    private readonly Dictionary<BAgent, bool> _makeTurnFlags = new Dictionary<BAgent, bool>();
    private BAgent[] _agents;
    public BAcademy Academy { get; set; }
    public int SessionIndex { get; set; }
    public int TurnIndex { get; private set; }

    public BAgent[] Agents => _agents;

    private void Awake()
    {
      _agents = GetComponentsInChildren<BAgent>(true);
      foreach (BAgent agent in _agents)
      {
        agent.Environment = this;
      }

      InitializeFlags(_endEpisodeFlags);
      InitializeFlags(_makeTurnFlags);
    }

    public void EndEpisode(BAgent agent)
    {
      _endEpisodeFlags[agent] = true;

      CheckAllAgentsEndEpisode();
    }

    private void CheckAllAgentsEndEpisode()
    {
      bool allAgentsEndedEpisode = true;
      foreach (KeyValuePair<BAgent, bool> flag in _endEpisodeFlags)
      {
        if (!flag.Value)
          allAgentsEndedEpisode = false;
      }

      if (allAgentsEndedEpisode)
      {
        Academy.CompleteEpisode(this);
        
        TurnIndex = 0;
        SessionIndex = Academy.StartSimulationCount;
        InitializeFlags(_endEpisodeFlags);
      }
    }

    public void MakeTurn(BAgent agent)
    {
      _makeTurnFlags[agent] = true;
      TurnIndex++;

      CheckAllAgentsMadeTurn();
    }

    private void CheckAllAgentsMadeTurn()
    {
      bool allAgentsMadeTurn = true;
      foreach (KeyValuePair<BAgent, bool> flag in _makeTurnFlags)
      {
        if (!flag.Value)
          allAgentsMadeTurn = false;
      }

      if (allAgentsMadeTurn)
      {
        InitializeFlags(_makeTurnFlags);
      }
    }

    private void InitializeFlags(Dictionary<BAgent, bool> flags)
    {
      flags.Clear();
      foreach (BAgent agent in _agents)
      {
        flags.Add(agent, false);
      }
    }
  }
}