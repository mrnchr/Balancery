using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Mrnchr.Balancery.Statistics.Database;

namespace Mrnchr.Balancery.Statistics
{
  public class DatabaseProvider : IDatabaseProvider
  {
    private readonly DataOptions _options;
    private readonly StatisticsDatabaseConnection _connection;
    private readonly SessionMetricData _cacheSessionMetric;
    private readonly TurnMetricData _cacheTurnMetric;
    private readonly ActionData _cacheAction;
    private readonly StartOptionData _cacheStartOption;

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
  }
}