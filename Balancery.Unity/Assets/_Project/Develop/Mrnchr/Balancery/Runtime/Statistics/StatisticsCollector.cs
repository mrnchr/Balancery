using System;
using System.Collections.Generic;

namespace Mrnchr.Balancery.Runtime.Statistics
{
  [Serializable]
  public class StatisticsCollector
  {
    private readonly List<Dictionary<string, float>> _statistics = new List<Dictionary<string, float>>();

    public List<Dictionary<string, float>> Statistics => _statistics;

    public void StartStatisticsEpisode()
    {
      Statistics.Add(new Dictionary<string, float>());
    }

    public void AddStatisticsItem(string key, float value)
    {
      Statistics[^1][key] = value;
    }
  }
}