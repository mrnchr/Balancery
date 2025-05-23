using LinqToDB.Mapping;

namespace Mrnchr.Balancery.Statistics.Database
{
  [Table("turn_metric")]
  public class TurnMetricData
  {
    [PrimaryKey, Column("session_number")] public int SessionNumber { get; set; }
    [PrimaryKey, Column("turn_number")] public int TurnNumber { get; set; }
    [PrimaryKey, Column("metric_name")] public string MetricName { get; set; }
    [Column("metric_value")] public float MetricValue { get; set; }
  }
}