using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;

namespace Mrnchr.Balancery.Statistics.Database
{
  public class SQLiteProvider : IDataProvider
  {
    private const string COLUMN_SESSION_NAME = "Session Number";
    private const string COLUMN_TURN_NAME = "Turn Number";

    private readonly StatisticsDatabaseConnection _connection;
    private readonly SessionMetricData _cacheSessionMetric;
    private readonly TurnMetricData _cacheTurnMetric;
    private readonly ActionData _cacheAction;
    private readonly StartOptionData _cacheStartOption;

    public StatisticsDatabaseConnection Connection => _connection;

    public SQLiteProvider(DataOptions options)
    {
      _connection = new StatisticsDatabaseConnection(options);
      _cacheSessionMetric = new SessionMetricData();
      _cacheTurnMetric = new TurnMetricData();
      _cacheAction = new ActionData();
      _cacheStartOption = new StartOptionData();
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
      _cacheSessionMetric.SessionNumber = sessionNumber;
      _cacheSessionMetric.MetricName = metricName;
      ((IData)_cacheSessionMetric).SetValue(value);

      _connection.InsertOrReplace(_cacheSessionMetric);
    }

    public async void RecordSessionMetricAsync<TType>(int sessionNumber, string metricName, TType value)
    {
      _cacheSessionMetric.SessionNumber = sessionNumber;
      _cacheSessionMetric.MetricName = metricName;
      ((IData)_cacheSessionMetric).SetValue(value);

      await _connection.InsertOrReplaceAsync(_cacheSessionMetric);
    }

    public void RecordTurnMetric<TType>(int sessionNumber, int turnNumber, string metricName, TType value)
    {
      _cacheTurnMetric.SessionNumber = sessionNumber;
      _cacheTurnMetric.TurnNumber = turnNumber;
      _cacheTurnMetric.MetricName = metricName;
      ((IData)_cacheTurnMetric).SetValue(value);

      _connection.InsertOrReplace(_cacheTurnMetric);
    }

    public async void RecordTurnMetricAsync<TType>(int sessionNumber, int turnNumber, string metricName, TType value)
    {
      _cacheTurnMetric.SessionNumber = sessionNumber;
      _cacheTurnMetric.TurnNumber = turnNumber;
      _cacheTurnMetric.MetricName = metricName;
      ((IData)_cacheTurnMetric).SetValue(value);

      await _connection.InsertOrReplaceAsync(_cacheTurnMetric);
    }

    public void RecordActionValue(int sessionNumber, int turnNumber, int actionIndex, float value)
    {
      _cacheAction.SessionNumber = sessionNumber;
      _cacheAction.TurnNumber = turnNumber;
      _cacheAction.ActionIndex = actionIndex;
      _cacheAction.ActionValue = value;

      _connection.InsertOrReplace(_cacheAction);
    }

    public async void RecordActionValueAsync(int sessionNumber, int turnNumber, int actionIndex, float value)
    {
      _cacheAction.SessionNumber = sessionNumber;
      _cacheAction.TurnNumber = turnNumber;
      _cacheAction.ActionIndex = actionIndex;
      _cacheAction.ActionValue = value;

      await _connection.InsertOrReplaceAsync(_cacheAction);
    }

    public void RecordOptionValue<TType>(int sessionNumber, string optionName, TType value)
    {
      _cacheStartOption.SessionNumber = sessionNumber;
      _cacheStartOption.OptionName = optionName;
      ((IData)_cacheStartOption).SetValue(value);

      _connection.InsertOrReplace(_cacheStartOption);
    }
    
    public async void RecordOptionValueAsync<TType>(int sessionNumber, string optionName, TType value)
    {
      _cacheStartOption.SessionNumber = sessionNumber;
      _cacheStartOption.OptionName = optionName;
      ((IData)_cacheStartOption).SetValue(value);

      await _connection.InsertOrReplaceAsync(_cacheStartOption);
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

    public async void ReplaceSessionNumberAsync(int oldSessionNumber, int newSessionNumber)
    {
      Task<int> sessionTask = _connection.SessionMetricTable.Where(x => x.SessionNumber == oldSessionNumber)
        .Set(x => x.SessionNumber, newSessionNumber).UpdateAsync();
      Task<int> turnTask = _connection.TurnMetricTable.Where(x => x.SessionNumber == oldSessionNumber)
        .Set(x => x.SessionNumber, newSessionNumber).UpdateAsync();
      Task<int> actionTask = _connection.ActionTable.Where(x => x.SessionNumber == oldSessionNumber)
        .Set(x => x.SessionNumber, newSessionNumber).UpdateAsync();
      
      await Task.WhenAll(sessionTask, turnTask, actionTask);
    }
    
    public void RemoveSession(int sessionNumber)
    {
      _connection.SessionMetricTable.Where(x => x.SessionNumber == sessionNumber).Delete();
      _connection.TurnMetricTable.Where(x => x.SessionNumber == sessionNumber).Delete();
      _connection.ActionTable.Where(x => x.SessionNumber == sessionNumber).Delete();
      _connection.StartOptionTable.Where(x => x.SessionNumber == sessionNumber).Delete();
    }

    public async void RemoveSessionAsync(int sessionNumber)
    {
      Task<int> sessionTask = _connection.SessionMetricTable.Where(x => x.SessionNumber == sessionNumber).DeleteAsync();
      Task<int> turnTask = _connection.TurnMetricTable.Where(x => x.SessionNumber == sessionNumber).DeleteAsync();
      Task<int> actionTask = _connection.ActionTable.Where(x => x.SessionNumber == sessionNumber).DeleteAsync();
      Task<int> optionTask = _connection.StartOptionTable.Where(x => x.SessionNumber == sessionNumber).DeleteAsync();
      
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