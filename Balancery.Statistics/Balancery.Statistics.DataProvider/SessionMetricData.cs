using LinqToDB.Mapping;

namespace Mrnchr.Balancery.Statistics.Database
{
  [Table("session_metric")]
  public class SessionMetricData : IData
  {
    [PrimaryKey, Column("session_number")]
    public int SessionNumber { get; set; }

    [PrimaryKey, Column("metric_name")]
    public string MetricName { get; set; }

    [Column("metric_type")]
    public int ValueType { get; set; }

    [Column("metric_value_real")]
    public float RealValue { get; set; }

    [Column("metric_value_string")]
    public string StringValue { get; set; }
  }
}