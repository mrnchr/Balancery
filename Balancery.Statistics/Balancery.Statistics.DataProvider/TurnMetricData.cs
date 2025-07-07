using System.Data.Common;
using LinqToDB.Mapping;

namespace Mrnchr.Balancery.Statistics.Database
{
  [Table("turn_metric")]
  public class TurnMetricData : IData
  {
    [PrimaryKey, Column("session_number")]
    public int SessionNumber { get; set; }

    [PrimaryKey, Column("turn_number")]
    public int TurnNumber { get; set; }

    [PrimaryKey, Column("metric_name")]
    public string MetricName { get; set; }

    [Column("value_type")]
    public int ValueType { get; set; }

    [Column("real_value")]
    public float RealValue { get; set; }

    [Column("string_value")]
    public string StringValue { get; set; }
    
    public void PrepareCommand(DbCommand command)
    {
      command.Parameters[0].Value = SessionNumber;
      command.Parameters[1].Value = TurnNumber;
      command.Parameters[2].Value = MetricName;
      command.Parameters[3].Value = ValueType;
      command.Parameters[4].Value = RealValue;
      command.Parameters[5].Value = StringValue;
    }
  }
}