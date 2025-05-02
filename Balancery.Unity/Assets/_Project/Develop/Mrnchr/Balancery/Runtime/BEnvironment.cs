using System.Collections.Generic;
using UnityEngine;

namespace Mrnchr.Balancery.Runtime
{
  public class BEnvironment : MonoBehaviour
  {
    private readonly Dictionary<BAgent, bool> _endEpisodeFlags = new Dictionary<BAgent, bool>();
    private BAgent[] _agents;
    public BAcademy Academy { get; set; }
    
    public BAgent[] Agents => _agents;

    private void Awake()
    {
      _agents = GetComponentsInChildren<BAgent>(true);
      foreach (BAgent agent in _agents)
      {
        agent.Environment = this;
      }
      
      InitializeFlags();
    }

    public void EndEpisode(BAgent agent)
    {
      _endEpisodeFlags[agent] = true;
      
      CheckAllAgentsEndEpisode();
    }

    private void CheckAllAgentsEndEpisode()
    {
      bool allAgentsEndedEpisode = true;
      foreach (KeyValuePair<BAgent,bool> flag in _endEpisodeFlags)
      {
        if(!flag.Value)
          allAgentsEndedEpisode = false;
      }
      
      if (allAgentsEndedEpisode)
      {
        Academy.CompleteEpisode(this);
        InitializeFlags();
      }
    }

    private void InitializeFlags()
    {
      _endEpisodeFlags.Clear();
      foreach (BAgent agent in _agents)
      {
        _endEpisodeFlags.Add(agent, false);
      }
    }
  }
}