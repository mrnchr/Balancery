using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Mrnchr.Balancery.Runtime.Repetition;
#if BALANCERY_STATISTICS
using Mrnchr.Balancery.Statistics;
#endif

namespace Mrnchr.Balancery.Runtime.Statistics
{
  public class StatisticsBridge : IDisposable
  {
#if BALANCERY_STATISTICS
    private readonly BStatistics _statistics;
    public BStatistics Statistics => _statistics;
#endif

    public StatisticsBridge(IStatisticsConfig config)
    {
#if BALANCERY_STATISTICS
      _statistics = new BStatistics(config);
#endif
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordSessionMetric<TType>(int sessionIndex, string metricId, TType value)
    {
      if (!RepetitionPlayer.IsRepetition)
        Statistics.Collector.RecordSessionMetric(sessionIndex, metricId, value);
    }

    public async Task RecordSessionMetricAsync<TType>(int sessionIndex, string metricId, TType value,
      CancellationToken token = default(CancellationToken))
    {
#if BALANCERY_STATISTICS
      if (!RepetitionPlayer.IsRepetition)
        await Statistics.Collector.RecordSessionMetricAsync(sessionIndex, metricId, value, token);
#endif
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordActionValue(int sessionIndex, int turnIndex, int actionIndex, float value)
    {
      if (!RepetitionPlayer.IsRepetition)
        Statistics.Collector.RecordActionValue(sessionIndex, turnIndex, actionIndex, value);
    }

    public async Task RecordActionValueAsync(int sessionIndex, int turnIndex, int actionIndex, float value,
      CancellationToken token = default(CancellationToken))
    {
#if BALANCERY_STATISTICS
      if (!RepetitionPlayer.IsRepetition)
        await Statistics.Collector.RecordActionValueAsync(sessionIndex, turnIndex, actionIndex, value, token);
#endif
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordTurnMetric<TType>(int sessionIndex, int turnIndex, string metricId, TType value)
    {
      if (!RepetitionPlayer.IsRepetition)
        Statistics.Collector.RecordTurnMetric(sessionIndex, turnIndex, metricId, value);
    }

    public async Task RecordTurnMetricAsync<TType>(int sessionIndex, int turnIndex, string metricId, TType value,
      CancellationToken token = default(CancellationToken))
    {
#if BALANCERY_STATISTICS
      if (!RepetitionPlayer.IsRepetition)
        await Statistics.Collector.RecordTurnMetricAsync(sessionIndex, turnIndex, metricId, value, token);
#endif
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordOptionValue<TType>(int sessionIndex, string optionId, TType value)
    {
      if (!RepetitionPlayer.IsRepetition)
        Statistics.DataProvider.RecordOptionValue(sessionIndex, optionId, value);
    }

    public async Task RecordOptionValueAsync<TType>(int sessionIndex, string optionId, TType value,
      CancellationToken token = default(CancellationToken))
    {
#if BALANCERY_STATISTICS
      if (!RepetitionPlayer.IsRepetition)
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
      if (!RepetitionPlayer.IsRepetition)
        Statistics.Export();
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RemoveSession(int sessionIndex)
    {
      if (!RepetitionPlayer.IsRepetition)
        Statistics.DataProvider.RemoveSession(sessionIndex);
    }

    public async Task RemoveSessionAsync(int sessionIndex, CancellationToken token = default(CancellationToken))
    {
#if BALANCERY_STATISTICS
      if (!RepetitionPlayer.IsRepetition)
        await Statistics.DataProvider.RemoveSessionAsync(sessionIndex, token);
#endif
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void ReplaceSessionNumber(int oldSessionIndex, int newSessionIndex)
    {
      if (!RepetitionPlayer.IsRepetition)
        Statistics.DataProvider.ReplaceSessionNumber(oldSessionIndex, newSessionIndex);
    }

    public async Task ReplaceSessionNumberAsync(int oldSessionIndex, int newSessionIndex,
      CancellationToken token = default(CancellationToken))
    {
#if BALANCERY_STATISTICS
      if (!RepetitionPlayer.IsRepetition)
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