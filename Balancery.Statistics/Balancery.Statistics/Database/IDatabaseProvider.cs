using System;
using System.Collections.Generic;
using System.Data;

namespace Mrnchr.Balancery.Statistics.Database
{
  public interface IDatabaseProvider : IDisposable, IAsyncDisposable
  {
    void RecordMetricValue(int sessionNumber, string metricName, float value);
    void RecordMetricValueToTurn(int sessionNumber, int turnNumber, string metricName, float value);
    void RecordActionValue(int sessionNumber, int turnNumber, int actionIndex, float value);
    void RecordOptionValue(int sessionNumber, string optionName, float value);
    List<float> GetActions(int sessionIndex, int turnIndex);
    void ReplaceSessionNumber(int oldSessionNumber, int newSessionNumber);
    void RemoveSession(int sessionNumber);
    DataTable GetMetricsTable();
    DataTable GetTurnsTable();
  }
}