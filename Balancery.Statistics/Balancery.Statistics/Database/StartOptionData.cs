using LinqToDB.Mapping;

namespace Mrnchr.Balancery.Statistics.Database
{
  [Table("start_option")]
  public class StartOptionData
  {
    [PrimaryKey, Column("session_number")] public int SessionNumber { get; set; }
    [PrimaryKey, Column("option_name")] public string OptionName { get; set; }
    [Column("option_value")] public float OptionValue { get; set; }
  }
}