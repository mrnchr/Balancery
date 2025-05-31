using System.Collections.Generic;
using System.IO;
using Mrnchr.Balancery.Runtime.Repetition;
using Mrnchr.Balancery.Runtime.Statistics;
using Mrnchr.Balancery.Runtime.Statistics.Configuration;
using Mrnchr.Balancery.Statistics;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.Events;

namespace Mrnchr.Balancery.Runtime.Academy
{
  public class BAcademy : MonoBehaviour
  {
    private readonly List<BEnvironment> _environments = new List<BEnvironment>();
    private int _startSimulationCount;
    private int _endSimulationCount;

    public BAcademySettings Settings;
    public BalanceryStatisticsConfigAsset StatisticsConfig;

    public UnityAction<BEnvironment> OnEpisodeComplete;

    public StatisticsBridge Statistics { get; set; }
    public IActionProvider ActionProvider { get; set; }

    public int StartSimulationCount => _startSimulationCount;

    private void Awake()
    {
#if BALANCERY_STATISTICS
      IBalanceryStatisticsConfig rawConfig = StatisticsConfig.CreateConfig();
      if (RepetitionPlayer.IsRepetition)
      {
        rawConfig.DatabasePath = Path.Combine(Application.dataPath,
          Path.GetDirectoryName(RepetitionPlayer.DatabaseFile) ?? string.Empty);
        rawConfig.DatabaseName = Path.GetFileName(RepetitionPlayer.DatabaseFile);
        RepetitionPlayer.OnRepeat += RestartSimulation;
      }

      Statistics = new StatisticsBridge(rawConfig);
#else
      Statistics = new StatisticsBridge();
#endif

      ActionProvider = new ActionProvider(Statistics);
    }

    public void StartSimulation()
    {
      int envNumber = Settings.NumberOfEnvironments;
      if (RepetitionPlayer.IsRepetition)
        envNumber = 1;

      for (int i = 0; i < envNumber; i++)
      {
        BEnvironment environment = Instantiate(Settings.EnvironmentPrefab);
        environment.Academy = this;
        _environments.Add(environment);
        environment.SessionIndex = _startSimulationCount++;
      }
    }

    public bool CanContinueSimulation()
    {
      return !RepetitionPlayer.IsRepetition;
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
      if (!RepetitionPlayer.IsRepetition && _endSimulationCount >= Settings.NumberOfSimulations)
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

    private void RestartSimulation()
    {
      if (_environments.Count > 0)
        _environments[0].ContinueSimulation();
    }

    private void OnDestroy()
    {
      RepetitionPlayer.OnRepeat -= RestartSimulation;
      Statistics?.Dispose();
    }
  }
}