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
    public void RecordActionValue(int sessionIndex, int turnIndex, int actionIndex, float value)
    {
      if (RepetitionPlayer.IsRepetition)
        return;

      Statistics.Collector.RecordActionValue(sessionIndex, turnIndex, actionIndex, value);
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordTurnValue(int sessionIndex, int turnIndex, string metricId, float value)
    {
      if (RepetitionPlayer.IsRepetition)
        return;

      Statistics.Collector.RecordMetricValueToTurn(sessionIndex, turnIndex, metricId, value);
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void Export()
    {
      if (RepetitionPlayer.IsRepetition)
        return;

      Statistics.Export();
    }

    public List<float> GetActions(int sessionIndex, int turnIndex)
    {
      return
#if BALANCERY_STATISTICS
        Statistics.DbProvider.Connection.ActionTable
          .Where(x => x.SessionNumber == sessionIndex && x.TurnNumber == turnIndex)
          .OrderBy(x => x.TurnNumber)
          .Select(x => x.ActionValue)
          .ToList();
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