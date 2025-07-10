using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
#if BALANCERY_STATISTICS
using Mrnchr.Balancery.Statistics;
#endif

namespace Mrnchr.Balancery.Runtime.Statistics
{
  public class StatisticsBridge : IDisposable
  {
#if BALANCERY_STATISTICS
    private readonly IStatisticsConfig _config;

    private BStatistics _statistics;
    public BStatistics Statistics => _statistics ??= new BStatistics(_config);
#endif

    public bool IsEnabled;
    public bool IsRepetition;
    public bool IsLearning;

    public bool IsActive => IsEnabled && !IsRepetition && !IsLearning;

#if BALANCERY_STATISTICS
    public StatisticsBridge(IStatisticsConfig config)
    {
      _config = config;
    }
#endif

    public StatisticsBridge()
    {
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordSessionMetric<TType>(int sessionIndex, string metricId, TType value)
    {
#if BALANCERY_STATISTICS
      if (IsActive)
        Statistics.Collector.RecordSessionMetric(sessionIndex, metricId, value);
#endif
    }

    public async Task RecordSessionMetricAsync<TType>(int sessionIndex, string metricId, TType value,
      CancellationToken token = default(CancellationToken))
    {
#if BALANCERY_STATISTICS
      if (IsActive)
        await Statistics.Collector.RecordSessionMetricAsync(sessionIndex, metricId, value, token);
#endif
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordActionValue(int sessionIndex, int turnIndex, int actionIndex, float value)
    {
#if BALANCERY_STATISTICS
      if (IsActive)
        Statistics.Collector.RecordActionValue(sessionIndex, turnIndex, actionIndex, value);
#endif
    }

    public async Task RecordActionValueAsync(int sessionIndex, int turnIndex, int actionIndex, float value,
      CancellationToken token = default(CancellationToken))
    {
#if BALANCERY_STATISTICS
      if (IsActive)
        await Statistics.Collector.RecordActionValueAsync(sessionIndex, turnIndex, actionIndex, value, token);
#endif
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordTurnMetric<TType>(int sessionIndex, int turnIndex, string metricId, TType value)
    {
#if BALANCERY_STATISTICS
      if (IsActive)
        Statistics.Collector.RecordTurnMetric(sessionIndex, turnIndex, metricId, value);
#endif
    }

    public async Task RecordTurnMetricAsync<TType>(int sessionIndex, int turnIndex, string metricId, TType value,
      CancellationToken token = default(CancellationToken))
    {
#if BALANCERY_STATISTICS
      if (IsActive)
        await Statistics.Collector.RecordTurnMetricAsync(sessionIndex, turnIndex, metricId, value, token);
#endif
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordOptionValue<TType>(int sessionIndex, string optionId, TType value)
    {
#if BALANCERY_STATISTICS
      if (IsActive)
        Statistics.DataProvider.RecordOptionValue(sessionIndex, optionId, value);
#endif
    }

    public async Task RecordOptionValueAsync<TType>(int sessionIndex, string optionId, TType value,
      CancellationToken token = default(CancellationToken))
    {
#if BALANCERY_STATISTICS
      if (IsActive)
        await Statistics.DataProvider.RecordOptionValueAsync(sessionIndex, optionId, value, token);
#endif
    }

    public TType ReadOptionValue<TType>(int sessionIndex, string optionId)
    {
      return
#if BALANCERY_STATISTICS
        Statistics.DataProvider.ReadOptionValue<TType>(sessionIndex, optionId);
#else
      default(TType);
#endif
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void Export()
    {
#if BALANCERY_STATISTICS
      if (IsActive)
        Statistics.Export();
#endif
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RemoveSession(int sessionIndex)
    {
#if BALANCERY_STATISTICS
      if (IsActive)
        Statistics.DataProvider.RemoveSession(sessionIndex);
#endif
    }

    public async Task RemoveSessionAsync(int sessionIndex, CancellationToken token = default(CancellationToken))
    {
#if BALANCERY_STATISTICS
      if (IsActive)
        await Statistics.DataProvider.RemoveSessionAsync(sessionIndex, token);
#endif
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void ReplaceSessionNumber(int oldSessionIndex, int newSessionIndex)
    {
#if BALANCERY_STATISTICS
      if (IsActive)
        Statistics.DataProvider.ReplaceSessionNumber(oldSessionIndex, newSessionIndex);
#endif
    }

    public async Task ReplaceSessionNumberAsync(int oldSessionIndex, int newSessionIndex,
      CancellationToken token = default(CancellationToken))
    {
#if BALANCERY_STATISTICS
      if (IsActive)
        await Statistics.DataProvider.ReplaceSessionNumberAsync(oldSessionIndex, newSessionIndex, token);
#endif
    }

    public List<float> GetActions(int sessionIndex, int turnIndex)
    {
      return
#if BALANCERY_STATISTICS
        Statistics.DataProvider.GetActions(sessionIndex, turnIndex);
#else
      new List<float>();
#endif
    }

    public void Dispose()
    {
#if BALANCERY_STATISTICS
      _statistics?.Dispose();
#endif
    }
  }
}