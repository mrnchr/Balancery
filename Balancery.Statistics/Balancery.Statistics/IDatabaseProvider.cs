using System;

namespace Mrnchr.Balancery.Statistics
{
  public interface IDatabaseProvider : IDisposable, IAsyncDisposable
  {
    void RecordMetricValue(int sessionNumber, string metricName, float value);
    void RecordMetricValueToTurn(int sessionNumber, int turnNumber, string metricName, float value);
  }
}