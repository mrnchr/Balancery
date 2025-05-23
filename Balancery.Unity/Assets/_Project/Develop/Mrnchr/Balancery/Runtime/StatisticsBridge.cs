using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if BALANCERY_STATISTICS
using Mrnchr.Balancery.Statistics;
#endif

namespace Mrnchr.Balancery.Runtime
{
  public class StatisticsBridge
  {
#if BALANCERY_STATISTICS
    private readonly BalanceryStatistics _statistics;

    public bool IsRepetition { get; set; }

    public BalanceryStatistics Statistics => _statistics;

    public StatisticsBridge(BalanceryStatistics statistics)
    {
      _statistics = statistics;
    }
#endif

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordActionValue(int sessionIndex, int turnIndex, int actionIndex, float value)
    {
      if (IsRepetition)
        return;
        
      Statistics.Collector.RecordActionValue(sessionIndex, turnIndex, actionIndex, value);
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordTurnValue(int sessionIndex, int turnIndex, string metricId, float value)
    {
      if (IsRepetition)
        return;
      
      Statistics.Collector.RecordMetricValueToTurn(sessionIndex, turnIndex, metricId, value);
    }

    [Conditional("BALANCERY_STATISTICS")]
    public void Export()
    {
      if (IsRepetition)
        return;
      
      Statistics.Export();
    }

    public List<float> GetActions(int sessionIndex, int turnIndex)
    {
      return
#if BALANCERY_STATISTICS
        Statistics.DbProvider.Connection.ActionTable
          .Where(x => x.SessionNumber == sessionIndex && x.TurnNumber == turnIndex)
          .Select(x => x.ActionValue)
          .ToList();
#else
      new List<float>();
#endif
    }
  }
}