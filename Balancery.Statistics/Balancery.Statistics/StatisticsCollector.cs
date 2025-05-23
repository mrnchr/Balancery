using System;

namespace Mrnchr.Balancery.Statistics
{
  [Serializable]
  public class StatisticsCollector
  {
    private readonly IDatabaseProvider _dbProvider;
    
    public StatisticsCollector(IDatabaseProvider dbProvider)
    {
      _dbProvider = dbProvider;
    }

    public void RecordMetricValue(int sessionIndex, string metricId, float value)
    {
      _dbProvider.RecordMetricValue(sessionIndex, metricId, value);
    }

    public void RecordMetricValueToTurn(int sessionIndex, int turnIndex, string metricId, float value)
    {
      _dbProvider.RecordMetricValueToTurn(sessionIndex, turnIndex, metricId, value);
    }

    public void RecordActionValue(int sessionIndex, int turnIndex, int actionIndex, float value)
    {
      _dbProvider.RecordActionValue(sessionIndex, turnIndex, actionIndex, value);
    }

    public void RecordOptionValue(int sessionIndex, string optionId, float value)
    {
      _dbProvider.RecordOptionValue(sessionIndex, optionId, value);
    }
  }
}