using System;
using System.Threading;
using System.Threading.Tasks;
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

    public async Task RecordSessionMetricAsync<TType>(int sessionIndex, string metricId, TType value,
      CancellationToken token = default(CancellationToken))
    {
      await _dbProvider.RecordSessionMetricAsync(sessionIndex, metricId, value, token);
    }

    public void RecordTurnMetric<TType>(int sessionIndex, int turnIndex, string metricId, TType value)
    {
      _dbProvider.RecordTurnMetric(sessionIndex, turnIndex, metricId, value);
    }

    public async Task RecordTurnMetricAsync<TType>(int sessionIndex, int turnIndex, string metricId, TType value,
      CancellationToken token = default(CancellationToken))
    {
      await _dbProvider.RecordTurnMetricAsync(sessionIndex, turnIndex, metricId, value, token);
    }

    public void RecordActionValue(int sessionIndex, int turnIndex, int actionIndex, float value)
    {
      _dbProvider.RecordActionValue(sessionIndex, turnIndex, actionIndex, value);
    }

    public async Task RecordActionValueAsync(int sessionIndex, int turnIndex, int actionIndex, float value,
      CancellationToken token = default(CancellationToken))
    {
      await _dbProvider.RecordActionValueAsync(sessionIndex, turnIndex, actionIndex, value, token);
    }

    public void RecordOptionValue<TType>(int sessionIndex, string optionId, TType value)
    {
      _dbProvider.RecordOptionValue(sessionIndex, optionId, value);
    }

    public async Task RecordOptionValueAsync<TType>(int sessionIndex, string optionId, TType value,
      CancellationToken token = default(CancellationToken))
    {
      await _dbProvider.RecordOptionValueAsync(sessionIndex, optionId, value, token);
    }
  }
}