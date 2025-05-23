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

    public DatabaseProvider(DataOptions options)
    {
      _options = options;
      _connection = new StatisticsDatabaseConnection(_options);
      _cacheSessionMetric = new SessionMetricData();
      _cacheTurnMetric = new TurnMetricData();
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

      if (_connection.SessionMetricTable.Any(x => x.SessionNumber == sessionNumber && x.MetricName == metricName))
      {
        _connection.Update(_cacheSessionMetric);
      }
      else
      {
        _connection.SessionMetricTable.Insert(() => new SessionMetricData
        {
          SessionNumber = sessionNumber,
          MetricName = metricName,
          MetricValue = value
        });
      }
    }

    public void RecordMetricValueToTurn(int sessionNumber, int turnNumber, string metricName, float value)
    {
      _cacheTurnMetric.SessionNumber = sessionNumber;
      _cacheTurnMetric.TurnNumber = turnNumber;
      _cacheTurnMetric.MetricName = metricName;
      _cacheTurnMetric.MetricValue = value;

      if (_connection.TurnMetricTable.Any(x =>
        x.SessionNumber == sessionNumber && x.TurnNumber == turnNumber && x.MetricName == metricName))
      {
        _connection.Update(_cacheTurnMetric);
      }
      else
      {
        _connection.TurnMetricTable.Insert(() => new TurnMetricData
        {
          SessionNumber = sessionNumber,
          TurnNumber = turnNumber,
          MetricName = metricName,
          MetricValue = value
        });
      }
    }
  }
}