using LinqToDB;
using LinqToDB.Data;

namespace Mrnchr.Balancery.Statistics.Database
{
  public class StatisticsDatabaseConnection : DataConnection
  {
    public StatisticsDatabaseConnection(DataOptions dataOptions) : base(dataOptions)
    {
    }
    
    public ITable<SessionMetricData> SessionMetricTable => GetOrCreateTable<SessionMetricData>();
    public ITable<TurnMetricData> TurnMetricTable => GetOrCreateTable<TurnMetricData>();
    public ITable<ActionData> ActionTable => GetOrCreateTable<ActionData>();
    public ITable<StartOptionData> StartOptionTable => GetOrCreateTable<StartOptionData>();

    private ITable<T> GetOrCreateTable<T>()
    {
      return this.CreateTable<T>(tableOptions: TableOptions.CreateIfNotExists);
    }
  }
}