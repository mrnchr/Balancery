using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Mrnchr.Balancery.Statistics.Database
{
  public interface IDataProvider : IDisposable, IAsyncDisposable
  {
    void RecordSessionMetric<TType>(int sessionNumber, string metricName, TType value);
    Task RecordSessionMetricAsync<TType>(int sessionNumber, string metricName, TType value, CancellationToken token = default(CancellationToken));
    void RecordTurnMetric<TType>(int sessionNumber, int turnNumber, string metricName, TType value);
    Task RecordTurnMetricAsync<TType>(int sessionNumber, int turnNumber, string metricName, TType value, CancellationToken token = default(CancellationToken));
    void RecordActionValue(int sessionNumber, int turnNumber, int actionIndex, float value);
    Task RecordActionValueAsync(int sessionNumber, int turnNumber, int actionIndex, float value, CancellationToken token = default(CancellationToken));
    void RecordOptionValue<TType>(int sessionNumber, string optionName, TType value);
    Task RecordOptionValueAsync<TType>(int sessionNumber, string optionName, TType value, CancellationToken token = default(CancellationToken));
    TType ReadOptionValue<TType>(int sessionNumber, string optionName);
    List<float> GetActions(int sessionNumber, int turnNumber);
    void ReplaceSessionNumber(int oldSessionNumber, int newSessionNumber);
    Task ReplaceSessionNumberAsync(int oldSessionNumber, int newSessionNumber, CancellationToken token = default(CancellationToken));
    void RemoveSession(int sessionNumber);
    Task RemoveSessionAsync(int sessionNumber, CancellationToken token = default(CancellationToken));
    DataTable GetMetricsTable();
    DataTable GetTurnsTable();
  }
}