using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mrnchr.Balancery.Runtime.Repetition;
#if BALANCERY_STATISTICS
using Mrnchr.Balancery.Statistics;
#endif

namespace Mrnchr.Balancery.Runtime.Statistics
{
  public class StatisticsBridge : IDisposable
  {
#if BALANCERY_STATISTICS
    private readonly BalanceryStatistics _statistics;
    public BalanceryStatistics Statistics => _statistics;
#endif

    public StatisticsBridge(IBalanceryStatisticsConfig config)
    {
#if BALANCERY_STATISTICS
      _statistics = new BalanceryStatistics(config);
#endif
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordMetricValue<TType>(int sessionIndex, string metricId, TType value)
    {
      if (RepetitionPlayer.IsRepetition)
        return;

      Statistics.Collector.RecordMetricValue(sessionIndex, metricId, value);
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordActionValue(int sessionIndex, int turnIndex, int actionIndex, float value)
    {
      if (RepetitionPlayer.IsRepetition)
        return;

      Statistics.Collector.RecordActionValue(sessionIndex, turnIndex, actionIndex, value);
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordTurnValue<TType>(int sessionIndex, int turnIndex, string metricId, TType value)
    {
      if (RepetitionPlayer.IsRepetition)
        return;

      Statistics.Collector.RecordMetricValueToTurn(sessionIndex, turnIndex, metricId, value);
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordOptionValue<TType>(int sessionIndex, string optionId, TType value)
    {
      if (RepetitionPlayer.IsRepetition)
        return;

      Statistics.DbProvider.RecordOptionValue(sessionIndex, optionId, value);
    }

    public TType ReadOptionValue<TType>(int sessionIndex, string optionId)
    {
      return
#if BALANCERY_STATISTICS
        Statistics.DbProvider.ReadOptionValue<TType>(sessionIndex, optionId);
#else
      default(TType);
#endif
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void Export()
    {
      if (RepetitionPlayer.IsRepetition)
        return;

      Statistics.Export();
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RemoveSession(int sessionIndex)
    {
      if (RepetitionPlayer.IsRepetition)
        return;

      Statistics.DbProvider.RemoveSession(sessionIndex);
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void ReplaceSessionNumber(int oldSessionIndex, int newSessionIndex)
    {
      if (RepetitionPlayer.IsRepetition)
        return;

      Statistics.DbProvider.ReplaceSessionNumber(oldSessionIndex, newSessionIndex);
    }

    public List<float> GetActions(int sessionIndex, int turnIndex)
    {
      return
#if BALANCERY_STATISTICS
        Statistics.DbProvider.GetActions(sessionIndex, turnIndex);
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