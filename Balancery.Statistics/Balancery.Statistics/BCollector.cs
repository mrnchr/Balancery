using System;
using System.Threading;
using System.Threading.Tasks;
using Balancery.Statistics.Shared;
using Mrnchr.Balancery.Statistics.Database;

namespace Mrnchr.Balancery.Statistics
{
  [Serializable]
  public class BCollector
  {
    private readonly DataProviderShell _dataProviderShell;
    
    private IDataProvider DataProvider => _dataProviderShell.DataProvider;

    public BCollector(DataProviderShell dataProviderShell)
    {
      _dataProviderShell = dataProviderShell;
    }

    public void RecordSessionMetric<TType>(int sessionIndex, string metricId, TType value)
    {
      DataProvider?.RecordSessionMetric(sessionIndex, metricId, value);
    }

    public async Task RecordSessionMetricAsync<TType>(int sessionIndex, string metricId, TType value,
      CancellationToken token = default(CancellationToken))
    {
      if (DataProvider != null)
        await DataProvider.RecordSessionMetricAsync(sessionIndex, metricId, value, token);
    }

    public void RecordTurnMetric<TType>(int sessionIndex, int turnIndex, string metricId, TType value)
    {
      DataProvider?.RecordTurnMetric(sessionIndex, turnIndex, metricId, value);
    }

    public async Task RecordTurnMetricAsync<TType>(int sessionIndex, int turnIndex, string metricId, TType value,
      CancellationToken token = default(CancellationToken))
    {
      if (DataProvider != null)
        await DataProvider.RecordTurnMetricAsync(sessionIndex, turnIndex, metricId, value, token);
    }

    public void RecordActionValue(int sessionIndex, int turnIndex, int actionIndex, float value)
    {
      DataProvider?.RecordActionValue(sessionIndex, turnIndex, actionIndex, value);
    }

    public async Task RecordActionValueAsync(int sessionIndex, int turnIndex, int actionIndex, float value,
      CancellationToken token = default(CancellationToken))
    {
      if (DataProvider != null)
        await DataProvider.RecordActionValueAsync(sessionIndex, turnIndex, actionIndex, value, token);
    }

    public void RecordOptionValue<TType>(int sessionIndex, string optionId, TType value)
    {
      DataProvider?.RecordOptionValue(sessionIndex, optionId, value);
    }

    public async Task RecordOptionValueAsync<TType>(int sessionIndex, string optionId, TType value,
      CancellationToken token = default(CancellationToken))
    {
      if (DataProvider != null)
        await DataProvider.RecordOptionValueAsync(sessionIndex, optionId, value, token);
    }
  }
}