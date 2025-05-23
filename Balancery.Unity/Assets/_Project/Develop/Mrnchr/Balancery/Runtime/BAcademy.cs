using System.Collections.Generic;
using Mrnchr.Balancery.Runtime.Statistics.Configuration;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.Events;

namespace Mrnchr.Balancery.Runtime
{
  public class BAcademy : MonoBehaviour
  {
    private readonly List<BEnvironment> _environments = new List<BEnvironment>();
    private int _startSimulationCount;
    private int _endSimulationCount;

    public BAcademySettings Settings;
    public MetricsConfig MetricsMap;

    public UnityAction<BEnvironment> OnEpisodeComplete;

    public StatisticsBridge Statistics { get; set; }

    public int StartSimulationCount => _startSimulationCount;

    private void Start()
    {
      for (int i = 0; i < Settings.NumberOfEnvironments; i++)
      {
        var environment = Instantiate(Settings.EnvironmentPrefab);
        environment.Academy = this;
        _environments.Add(environment);
        environment.SessionIndex = _startSimulationCount++;
      }
    }

    public void CompleteEpisode(BEnvironment environment)
    {
      OnEpisodeComplete?.Invoke(environment);
      _endSimulationCount++;
      _startSimulationCount++;

      CheckAllSimulationsComplete();
    }

    private void CheckAllSimulationsComplete()
    {
      if (_endSimulationCount >= Settings.NumberOfSimulations)
      {
        foreach (var environment in _environments)
          Destroy(environment.gameObject);

        _environments.Clear();
        _endSimulationCount = 0;
      }
    }

    public void RecordActions(BAgent agent, ActionBuffers actions)
    {
      if (Statistics != null)
      {
        for (int i = 0; i < actions.ContinuousActions.Length + actions.DiscreteActions.Length; i++)
        {
          float value = i < actions.ContinuousActions.Length ? actions.ContinuousActions[i] : actions.DiscreteActions[i];
          Statistics.RecordActionValue(agent.Environment.SessionIndex, agent.Environment.TurnIndex, i, value);
        }
      }
    }
  }
}