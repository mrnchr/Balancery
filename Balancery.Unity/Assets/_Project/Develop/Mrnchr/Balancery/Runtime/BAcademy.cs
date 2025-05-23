using System;
using System.Collections.Generic;
using Mrnchr.Balancery.Runtime.Repetition;
using Mrnchr.Balancery.Runtime.Statistics.Configuration;
using Mrnchr.Balancery.Statistics;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.Events;

namespace Mrnchr.Balancery.Runtime
{
  public class BAcademy : MonoBehaviour
  {
    public static bool IsRepetition;
    
    private readonly List<BEnvironment> _environments = new List<BEnvironment>();
    private int _startSimulationCount;
    private int _endSimulationCount;

    public BAcademySettings Settings;
    public MetricsConfig MetricsMap;
    public BalanceryStatisticsConfigAsset StatisticsConfig;

    public UnityAction<BEnvironment> OnEpisodeComplete;

    public StatisticsBridge Statistics { get; set; }
    public IActionProvider ActionProvider { get; set; }

    public int StartSimulationCount => _startSimulationCount;

    private void Awake()
    {
#if BALANCERY_STATISTICS
      IBalanceryStatisticsConfig rawConfig = StatisticsConfig.CreateConfig();
      Statistics = new StatisticsBridge(new BalanceryStatistics(rawConfig));
      Statistics.IsRepetition = IsRepetition;
#else
      Statistics = new StatisticsBridge();
#endif
      
      ActionProvider = new ActionProvider(Statistics);
    }

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

        Statistics.Export();
      }
    }

    public void RecordActions(BAgent agent, ActionBuffers actions)
    {
      if (Statistics != null)
      {
        for (int i = 0; i < actions.ContinuousActions.Length + actions.DiscreteActions.Length; i++)
        {
          float value = i < actions.ContinuousActions.Length
            ? actions.ContinuousActions[i]
            : actions.DiscreteActions[i - actions.ContinuousActions.Length];
          Statistics.RecordActionValue(agent.Environment.SessionIndex, agent.Environment.TurnIndex, i, value);
        }
      }
    }
  }
}