using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;

namespace Mrnchr.Balancery.Statistics.Database
{
  public class SQLiteProvider : IDataProvider
  {
    private const string COLUMN_SESSION_NAME = "Session Number";
    private const string COLUMN_TURN_NAME = "Turn Number";

    private readonly StatisticsDatabaseConnection _connection;
    private readonly SessionMetricData _cachedSessionMetric;
    private readonly TurnMetricData _cachedTurnMetric;
    private readonly ActionData _cachedAction;
    private readonly StartOptionData _cachedStartOption;

    public StatisticsDatabaseConnection Connection => _connection;

    public SQLiteProvider(IStatisticsConfig config)
    {
      _connection = new StatisticsDatabaseConnection(new DataOptions()
        .UseSQLite($"Data Source = {Path.Combine(config.DataFilePath, config.DataFileName)}"));
      _cachedSessionMetric = new SessionMetricData();
      _cachedTurnMetric = new TurnMetricData();
      _cachedAction = new ActionData();
      _cachedStartOption = new StartOptionData();
    }

    public void Dispose()
    {
      _connection?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
      if (_connection != null)
        await _connection.DisposeAsync();
    }

    public void RecordSessionMetric<TType>(int sessionNumber, string metricName, TType value)
    {
      _cachedSessionMetric.SessionNumber = sessionNumber;
      _cachedSessionMetric.MetricName = metricName;
      ((IData)_cachedSessionMetric).SetValue(value);

      _connection.RecordSession(_cachedSessionMetric);
    }

    public async Task RecordSessionMetricAsync<TType>(int sessionNumber, string metricName, TType value,
      CancellationToken token = default(CancellationToken))
    {
      _cachedSessionMetric.SessionNumber = sessionNumber;
      _cachedSessionMetric.MetricName = metricName;
      ((IData)_cachedSessionMetric).SetValue(value);

      await _connection.RecordSessionAsync(_cachedSessionMetric, token);
    }

    public void RecordTurnMetric<TType>(int sessionNumber, int turnNumber, string metricName, TType value)
    {
      _cachedTurnMetric.SessionNumber = sessionNumber;
      _cachedTurnMetric.TurnNumber = turnNumber;
      _cachedTurnMetric.MetricName = metricName;
      ((IData)_cachedTurnMetric).SetValue(value);

      _connection.RecordTurn(_cachedTurnMetric);
    }

    public async Task RecordTurnMetricAsync<TType>(int sessionNumber, int turnNumber, string metricName, TType value,
      CancellationToken token = default(CancellationToken))
    {
      _cachedTurnMetric.SessionNumber = sessionNumber;
      _cachedTurnMetric.TurnNumber = turnNumber;
      _cachedTurnMetric.MetricName = metricName;
      ((IData)_cachedTurnMetric).SetValue(value);
      
      await _connection.RecordTurnAsync(_cachedTurnMetric, token);
    }

    public void RecordActionValue(int sessionNumber, int turnNumber, int actionIndex, float value)
    {
      _cachedAction.SessionNumber = sessionNumber;
      _cachedAction.TurnNumber = turnNumber;
      _cachedAction.ActionIndex = actionIndex;
      _cachedAction.ActionValue = value;

      _connection.RecordAction(_cachedAction);
    }

    public async Task RecordActionValueAsync(int sessionNumber, int turnNumber, int actionIndex, float value,
      CancellationToken token = default(CancellationToken))
    {
      _cachedAction.SessionNumber = sessionNumber;
      _cachedAction.TurnNumber = turnNumber;
      _cachedAction.ActionIndex = actionIndex;
      _cachedAction.ActionValue = value;

      await _connection.RecordActionAsync(_cachedAction, token);
    }

    public void RecordOptionValue<TType>(int sessionNumber, string optionName, TType value)
    {
      _cachedStartOption.SessionNumber = sessionNumber;
      _cachedStartOption.OptionName = optionName;
      ((IData)_cachedStartOption).SetValue(value);

      _connection.RecordOption(_cachedStartOption);
    }

    public async Task RecordOptionValueAsync<TType>(int sessionNumber, string optionName, TType value,
      CancellationToken token = default(CancellationToken))
    {
      _cachedStartOption.SessionNumber = sessionNumber;
      _cachedStartOption.OptionName = optionName;
      ((IData)_cachedStartOption).SetValue(value);
      
      await _connection.RecordOptionAsync(_cachedStartOption, token);
    }

    public TType ReadOptionValue<TType>(int sessionNumber, string optionName)
    {
      return _connection.StartOptionTable
        .Where(x => x.SessionNumber == sessionNumber && x.OptionName == optionName)
        .Select(x => ((IData)x).GetValue<TType>()).First();
    }

    public List<float> GetActions(int sessionNumber, int turnNumber)
    {
      return Connection.ActionTable
        .Where(x => x.SessionNumber == sessionNumber && x.TurnNumber == turnNumber)
        .OrderBy(x => x.TurnNumber)
        .ThenBy(x => x.ActionIndex)
        .Select(x => x.ActionValue)
        .ToList();
    }

    public void ReplaceSessionNumber(int oldSessionNumber, int newSessionNumber)
    {
      _connection.SessionMetricTable.Where(x => x.SessionNumber == oldSessionNumber)
        .Set(x => x.SessionNumber, newSessionNumber).Update();
      _connection.TurnMetricTable.Where(x => x.SessionNumber == oldSessionNumber)
        .Set(x => x.SessionNumber, newSessionNumber).Update();
      _connection.ActionTable.Where(x => x.SessionNumber == oldSessionNumber)
        .Set(x => x.SessionNumber, newSessionNumber).Update();
    }

    public async Task ReplaceSessionNumberAsync(int oldSessionNumber, int newSessionNumber,
      CancellationToken token = default(CancellationToken))
    {
      Task<int> sessionTask = _connection.SessionMetricTable.Where(x => x.SessionNumber == oldSessionNumber)
        .Set(x => x.SessionNumber, newSessionNumber).UpdateAsync(token: token);
      Task<int> turnTask = _connection.TurnMetricTable.Where(x => x.SessionNumber == oldSessionNumber)
        .Set(x => x.SessionNumber, newSessionNumber).UpdateAsync(token: token);
      Task<int> actionTask = _connection.ActionTable.Where(x => x.SessionNumber == oldSessionNumber)
        .Set(x => x.SessionNumber, newSessionNumber).UpdateAsync(token: token);

      await Task.WhenAll(sessionTask, turnTask, actionTask);
    }

    public void RemoveSession(int sessionNumber)
    {
      _connection.SessionMetricTable.Where(x => x.SessionNumber == sessionNumber).Delete();
      _connection.TurnMetricTable.Where(x => x.SessionNumber == sessionNumber).Delete();
      _connection.ActionTable.Where(x => x.SessionNumber == sessionNumber).Delete();
      _connection.StartOptionTable.Where(x => x.SessionNumber == sessionNumber).Delete();
    }

    public async Task RemoveSessionAsync(int sessionNumber, CancellationToken token = default(CancellationToken))
    {
      Task<int> sessionTask = _connection.SessionMetricTable.Where(x => x.SessionNumber == sessionNumber)
        .DeleteAsync(token: token);
      Task<int> turnTask = _connection.TurnMetricTable.Where(x => x.SessionNumber == sessionNumber)
        .DeleteAsync(token: token);
      Task<int> actionTask = _connection.ActionTable.Where(x => x.SessionNumber == sessionNumber)
        .DeleteAsync(token: token);
      Task<int> optionTask = _connection.StartOptionTable.Where(x => x.SessionNumber == sessionNumber)
        .DeleteAsync(token: token);

      await Task.WhenAll(sessionTask, turnTask, actionTask, optionTask);
    }

    public DataTable GetMetricsTable()
    {
      DataTable table = new DataTable();
      table.Columns.Add(COLUMN_SESSION_NAME);
      IQueryable<string> columns = _connection.SessionMetricTable.Select(x => x.MetricName).Distinct().OrderBy(x => x);
      foreach (string columnName in columns)
        table.Columns.Add(columnName);

      List<SessionMetricData> metrics = _connection.SessionMetricTable
        .OrderBy(x => x.SessionNumber).ToList();
      int k = -1;
      int current = -1;
      foreach (SessionMetricData metric in metrics)
      {
        if (metric.SessionNumber != current)
        {
          current = metric.SessionNumber;
          k++;
          DataRow row = table.NewRow();
          table.Rows.Add(row);
        }

        table.Rows[k][COLUMN_SESSION_NAME] = metric.SessionNumber;
        table.Rows[k][metric.MetricName] = ((IData)metric).GetValue();
      }

      return table;
    }

    public DataTable GetTurnsTable()
    {
      DataTable table = new DataTable();
      table.Columns.Add(COLUMN_SESSION_NAME);
      table.Columns.Add(COLUMN_TURN_NAME);
      IQueryable<string> columns = _connection.TurnMetricTable.Select(x => x.MetricName).Distinct().OrderBy(x => x);
      foreach (string columnName in columns)
        table.Columns.Add(columnName);

      var metrics = _connection.TurnMetricTable
        .OrderBy(x => x.SessionNumber)
        .ThenBy(x => x.TurnNumber).ToList();
      int k = -1;
      int session = -1;
      int turn = -1;
      foreach (var metric in metrics)
      {
        if (metric.SessionNumber != session)
        {
          session = metric.SessionNumber;
          k++;
          DataRow row = table.NewRow();
          table.Rows.Add(row);
          table.Rows[k][COLUMN_SESSION_NAME] = metric.SessionNumber;
        }

        if (metric.TurnNumber != turn)
        {
          turn = metric.TurnNumber;
          k++;
          DataRow row = table.NewRow();
          table.Rows.Add(row);
        }

        table.Rows[k][COLUMN_TURN_NAME] = metric.TurnNumber;
        table.Rows[k][metric.MetricName] = ((IData)metric).GetValue();
      }

      return table;
    }
  }
}