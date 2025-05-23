using System.Diagnostics;
#if BALANCERY_STATISTICS
using Mrnchr.Balancery.Statistics;
#endif

namespace Mrnchr.Balancery.Runtime
{
  public class StatisticsBridge
  {
#if BALANCERY_STATISTICS
    private readonly BalanceryStatistics _statistics;

    public StatisticsBridge(BalanceryStatistics statistics)
    {
      _statistics = statistics;
    }
#endif

    [Conditional("BALANCERY_STATISTICS")]
    public void RecordActionValue(int sessionIndex, int turnIndex, int actionIndex, float value)
    {
      _statistics.Collector.RecordActionValue(sessionIndex, turnIndex, actionIndex, value);
    }
  }
}