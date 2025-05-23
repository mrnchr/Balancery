using System;
using System.Collections.Generic;

namespace Mrnchr.Balancery.Statistics
{
  [Serializable]
  public class StatisticsCollector
  {
    private readonly IDatabaseProvider _dbProvider;
    
    private int _sessionCounter;
    private int _turnCounter;
    
    public StatisticsCollector(IDatabaseProvider dbProvider)
    {
      _dbProvider = dbProvider;
    }

    public void NextSession()
    {
      _sessionCounter++;
      _turnCounter = 0;
    }

    public void NextTurn()
    {
      _turnCounter++;
    }

    public void RecordMetricValue(string metricId, float value)
    {
      _dbProvider.RecordMetricValue(_sessionCounter, metricId, value);
    }

    public void RecordMetricValueToTurn(string metricId, float value)
    {
      _dbProvider.RecordMetricValueToTurn(_sessionCounter, _turnCounter, metricId, value);
    }
  }
}