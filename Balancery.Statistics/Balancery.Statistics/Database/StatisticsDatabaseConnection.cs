using LinqToDB;
using LinqToDB.Data;

namespace Mrnchr.Balancery.Statistics.Database
{
  public class StatisticsDatabaseConnection : DataConnection
  {
    public StatisticsDatabaseConnection(DataOptions dataOptions) : base(dataOptions)
    {
      this.CreateTable<SessionMetricData>(tableOptions: TableOptions.CreateIfNotExists);
      this.CreateTable<TurnMetricData>(tableOptions: TableOptions.CreateIfNotExists);
      this.CreateTable<ActionData>(tableOptions: TableOptions.CreateIfNotExists);
      this.CreateTable<StartOptionData>(tableOptions: TableOptions.CreateIfNotExists);
    }
    
    public ITable<SessionMetricData> SessionMetricTable => this.GetTable<SessionMetricData>();
    public ITable<TurnMetricData> TurnMetricTable => this.GetTable<TurnMetricData>();
    public ITable<ActionData> ActionTable => this.GetTable<ActionData>();
    public ITable<StartOptionData> StartOptionTable => this.GetTable<StartOptionData>();
  }
}