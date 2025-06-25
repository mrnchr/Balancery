using System;
using System.Collections.Generic;
using System.Data;

namespace Mrnchr.Balancery.Statistics.Database
{
  public interface IDataProvider : IDisposable, IAsyncDisposable
  {
    void RecordSessionMetric<TType>(int sessionNumber, string metricName, TType value);
    void RecordSessionMetricAsync<TType>(int sessionNumber, string metricName, TType value);
    void RecordTurnMetric<TType>(int sessionNumber, int turnNumber, string metricName, TType value);
    void RecordTurnMetricAsync<TType>(int sessionNumber, int turnNumber, string metricName, TType value);
    void RecordActionValue(int sessionNumber, int turnNumber, int actionIndex, float value);
    void RecordActionValueAsync(int sessionNumber, int turnNumber, int actionIndex, float value);
    void RecordOptionValue<TType>(int sessionNumber, string optionName, TType value);
    void RecordOptionValueAsync<TType>(int sessionNumber, string optionName, TType value);
    TType ReadOptionValue<TType>(int sessionNumber, string optionName);
    List<float> GetActions(int sessionNumber, int turnNumber);
    void ReplaceSessionNumber(int oldSessionNumber, int newSessionNumber);
    void ReplaceSessionNumberAsync(int oldSessionNumber, int newSessionNumber);
    void RemoveSession(int sessionNumber);
    void RemoveSessionAsync(int sessionNumber);
    DataTable GetMetricsTable();
    DataTable GetTurnsTable();
  }
}