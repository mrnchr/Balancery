using LinqToDB.Mapping;

namespace Mrnchr.Balancery.Statistics.Database
{
  [Table("action")]
  public class ActionData
  {
    [PrimaryKey, Column("session_number")] public int SessionNumber { get; set; }
    [PrimaryKey, Column("turn_number")] public int TurnNumber { get; set; }
    [PrimaryKey, Column("action_index")] public int ActionIndex { get; set; }
    [Column("action_value")] public float ActionValue { get; set; }
  }
}