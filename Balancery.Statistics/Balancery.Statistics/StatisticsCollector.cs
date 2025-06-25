using System;
using Mrnchr.Balancery.Statistics.Database;

namespace Mrnchr.Balancery.Statistics
{
  [Serializable]
  public class StatisticsCollector
  {
    private readonly IDataProvider _dbProvider;
    
    public StatisticsCollector(IDataProvider dbProvider)
    {
      _dbProvider = dbProvider;
    }

    public void RecordSessionMetric<TType>(int sessionIndex, string metricId, TType value)
    {
      _dbProvider.RecordSessionMetric(sessionIndex, metricId, value);
    }

    public void RecordTurnMetric<TType>(int sessionIndex, int turnIndex, string metricId, TType value)
    {
      _dbProvider.RecordTurnMetric(sessionIndex, turnIndex, metricId, value);
    }

    public void RecordActionValue(int sessionIndex, int turnIndex, int actionIndex, float value)
    {
      _dbProvider.RecordActionValue(sessionIndex, turnIndex, actionIndex, value);
    }

    public void RecordOptionValue<TType>(int sessionIndex, string optionId, TType value)
    {
      _dbProvider.RecordOptionValue(sessionIndex, optionId, value);
    }
  }
}