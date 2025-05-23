using LinqToDB.Mapping;

namespace Mrnchr.Balancery.Statistics.Database
{
  [Table("session_metric")]
  public class SessionMetricData
  {
    [PrimaryKey, Column("session_number")] public int SessionNumber { get; set; }
    [PrimaryKey, Column("metric_name")] public string MetricName { get; set; }
    [Column("metric_value")] public float MetricValue { get; set; }
  }
}