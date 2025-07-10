using System.Collections.Generic;
using Mrnchr.Balancery.Runtime.Repetition;
using Mrnchr.Balancery.Runtime.Statistics;
using Mrnchr.Balancery.Runtime.Statistics.Configuration;
using Unity.MLAgents.Actuators;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;

namespace Mrnchr.Balancery.Runtime.Academy
{
  public class BAcademy : MonoBehaviour
  {
    private static ProfilerMarker _profilerMarker = new ProfilerMarker("RecordActionValue");

    private readonly List<BEnvironment> _environments = new List<BEnvironment>();
    private readonly List<int> _completedEpisodes = new List<int>();

    public int _startedSimulationCount;
    public int _completedSimulationCount;

    public BAcademySettings Settings;
    public BStatisticsAsset StatisticsConfig;

    public UnityAction<BEnvironment> OnEpisodeComplete;

    public StatisticsBridge Statistics { get; set; }
    public IActionProvider ActionProvider { get; set; }

    public int StartedSimulationCount => _startedSimulationCount;

    private void Awake()
    {
#if BALANCERY_STATISTICS
      IStatisticsConfig rawConfig = StatisticsConfig.CreateConfig();
      if (RepetitionPlayer.IsRepetition)
      {
        rawConfig.DataFilePath = Path.Combine(Application.dataPath,
          Path.GetDirectoryName(RepetitionPlayer.DatabaseFile) ?? string.Empty);
        rawConfig.DataFileName = Path.GetFileName(RepetitionPlayer.DatabaseFile);
        RepetitionPlayer.OnRepeat += RestartSimulation;
      }

      Statistics = new StatisticsBridge(rawConfig);
#else
      Statistics = new StatisticsBridge();
#endif

      ActionProvider = new ActionProvider(Statistics);

      Statistics.IsEnabled = StatisticsConfig.EnableStatistics;
      Statistics.IsRepetition = RepetitionPlayer.IsRepetition;
      Statistics.IsLearning = Unity.MLAgents.Academy.Instance.IsCommunicatorOn;
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
        environment.SessionIndex = _startedSimulationCount++;
      }
    }

    public void CompleteEpisode(BEnvironment environment)
    {
      OnEpisodeComplete?.Invoke(environment);
      _completedEpisodes.Add(environment.SessionIndex);
      _completedSimulationCount++;
      _startedSimulationCount++;

      CheckAllSimulationsComplete();
    }

    private void CheckAllSimulationsComplete()
    {
      if (!Statistics.IsRepetition && !Statistics.IsLearning
        && _completedSimulationCount >= Settings.NumberOfSimulations)
      {
        foreach (var environment in _environments)
          Destroy(environment.gameObject);

        _environments.Clear();
        _completedSimulationCount = 0;

        int k = 0;
        for (int i = 0; i < _startedSimulationCount; i++)
        {
          if (!_completedEpisodes.Contains(i))
            _ = Statistics.RemoveSessionAsync(i);
          else if (i != k)
            _ = Statistics.ReplaceSessionNumberAsync(i, k++);
          else
            k++;
        }

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
#if UNITY_EDITOR
          using (_profilerMarker.Auto())
#endif
          {
            _ = Statistics.RecordActionValueAsync(agent.Environment.SessionIndex, agent.Environment.TurnIndex, i,
              value);
          }
        }
      }
    }

    private void RestartSimulation()
    {
      if (_environments.Count > 0)
        _environments[0].ContinueSimulation();
    }

    private void Update()
    {
      Statistics.IsEnabled = StatisticsConfig.EnableStatistics;
      Statistics.IsRepetition = RepetitionPlayer.IsRepetition;
      Statistics.IsLearning = Unity.MLAgents.Academy.Instance.IsCommunicatorOn;
    }

    private void OnDestroy()
    {
      RepetitionPlayer.OnRepeat -= RestartSimulation;
      Statistics?.Dispose();
    }
  }
}