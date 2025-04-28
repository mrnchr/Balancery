using System.Collections.Generic;
using Mrnchr.Balancery.Runtime.Statistics;
using UnityEngine;
using UnityEngine.Events;

namespace Mrnchr.Balancery.Runtime
{
  public class BAcademy : MonoBehaviour
  {
    private readonly List<BEnvironment> _environments = new List<BEnvironment>();
    private int _simulationCount;
    
    public BAcademySettings Settings;

    public UnityAction<BEnvironment> OnEpisodeComplete;
    
    public StatisticsCollector StatisticsCollector { get; } = new StatisticsCollector();

    private void Start()
    {
      for (int i = 0; i < Settings.NumberOfEnvironments; i++)
      {
        var environment = Instantiate(Settings.EnvironmentPrefab);
        environment.Academy = this;
        _environments.Add(environment);
      }
    }

    public void CompleteEpisode(BEnvironment environment)
    {
      _simulationCount++;
      OnEpisodeComplete?.Invoke(environment);
      
      CheckAllSimulationsComplete();
    }

    private void CheckAllSimulationsComplete()
    {
      if (_simulationCount >= Settings.NumberOfSimulations)
      {
        foreach (var environment in _environments)
          Destroy(environment.gameObject);

        StatisticsAnalyzer analyzer = new StatisticsAnalyzer(StatisticsCollector.Statistics);
        analyzer.GenerateClusterStatistics(2);
        
        _environments.Clear();
        _simulationCount = 0;
      }
    }
  }
}