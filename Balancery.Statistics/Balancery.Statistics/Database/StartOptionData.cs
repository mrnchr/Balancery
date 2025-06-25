using LinqToDB.Mapping;

namespace Mrnchr.Balancery.Statistics.Database
{
  [Table("start_option")]
  public class StartOptionData : IData
  {
    [PrimaryKey, Column("session_number")]
    public int SessionNumber { get; set; }

    [PrimaryKey, Column("option_name")]
    public string OptionName { get; set; }

    [Column("option_type")]
    public int ValueType { get; set; }

    [Column("option_value_real")]
    public float RealValue { get; set; }

    [Column("option_value_string")]
    public string StringValue { get; set; }
  }
}