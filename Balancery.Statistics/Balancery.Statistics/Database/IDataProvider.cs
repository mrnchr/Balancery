using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Mrnchr.Balancery.Statistics.Database
{
  public interface IDataProvider : IDisposable, IAsyncDisposable
  {
    void RecordSessionMetric<TType>(int sessionNumber, string metricName, TType value);
    Task RecordSessionMetricAsync<TType>(int sessionNumber, string metricName, TType value);
    void RecordTurnMetric<TType>(int sessionNumber, int turnNumber, string metricName, TType value);
    Task RecordTurnMetricAsync<TType>(int sessionNumber, int turnNumber, string metricName, TType value);
    void RecordActionValue(int sessionNumber, int turnNumber, int actionIndex, float value);
    Task RecordActionValueAsync(int sessionNumber, int turnNumber, int actionIndex, float value);
    void RecordOptionValue<TType>(int sessionNumber, string optionName, TType value);
    Task RecordOptionValueAsync<TType>(int sessionNumber, string optionName, TType value);
    TType ReadOptionValue<TType>(int sessionNumber, string optionName);
    List<float> GetActions(int sessionNumber, int turnNumber);
    void ReplaceSessionNumber(int oldSessionNumber, int newSessionNumber);
    Task ReplaceSessionNumberAsync(int oldSessionNumber, int newSessionNumber);
    void RemoveSession(int sessionNumber);
    Task RemoveSessionAsync(int sessionNumber);
    DataTable GetMetricsTable();
    DataTable GetTurnsTable();
  }
}