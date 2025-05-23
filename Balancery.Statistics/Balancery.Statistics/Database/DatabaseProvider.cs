using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;

namespace Mrnchr.Balancery.Statistics.Database
{
  public class DatabaseProvider : IDatabaseProvider
  {
    private const string COLUMN_SESSION_NAME = "SessionNumber";
    private const string COLUMN_TURN_NAME = "TurnNumber";
    
    private readonly DataOptions _options;
    private readonly StatisticsDatabaseConnection _connection;
    private readonly SessionMetricData _cacheSessionMetric;
    private readonly TurnMetricData _cacheTurnMetric;
    private readonly ActionData _cacheAction;
    private readonly StartOptionData _cacheStartOption;

    public StatisticsDatabaseConnection Connection => _connection;

    public DatabaseProvider(DataOptions options)
    {
      _options = options;
      _connection = new StatisticsDatabaseConnection(_options);
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

    public void RecordMetricValue(int sessionNumber, string metricName, float value)
    {
      _cacheSessionMetric.SessionNumber = sessionNumber;
      _cacheSessionMetric.MetricName = metricName;
      _cacheSessionMetric.MetricValue = value;

      _connection.InsertOrReplace(_cacheSessionMetric);
    }

    public void RecordMetricValueToTurn(int sessionNumber, int turnNumber, string metricName, float value)
    {
      _cacheTurnMetric.SessionNumber = sessionNumber;
      _cacheTurnMetric.TurnNumber = turnNumber;
      _cacheTurnMetric.MetricName = metricName;
      _cacheTurnMetric.MetricValue = value;

      _connection.InsertOrReplace(_cacheTurnMetric);
    }

    public void RecordActionValue(int sessionNumber, int turnNumber, int actionIndex, float value)
    {
      _cacheAction.SessionNumber = sessionNumber;
      _cacheAction.TurnNumber = turnNumber;
      _cacheAction.ActionIndex = actionIndex;
      _cacheAction.ActionValue = value;

      _connection.InsertOrReplace(_cacheAction);
    }

    public void RecordOptionValue(int sessionNumber, string optionName, float value)
    {
      _cacheStartOption.SessionNumber = sessionNumber;
      _cacheStartOption.OptionName = optionName;
      _cacheStartOption.OptionValue = value;

      _connection.InsertOrReplace(_cacheStartOption);
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
        table.Rows[k][metric.MetricName] = metric.MetricValue;
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
        table.Rows[k][metric.MetricName] = metric.MetricValue;
      }

      return table;
    }
  }
}