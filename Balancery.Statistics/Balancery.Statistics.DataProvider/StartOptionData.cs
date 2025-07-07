using System.Data.Common;
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

    [Column("value_type")]
    public int ValueType { get; set; }

    [Column("real_value")]
    public float RealValue { get; set; }

    [Column("string_value")]
    public string StringValue { get; set; }

    public void PrepareCommand(DbCommand command)
    {
      command.Parameters[0].Value = SessionNumber;
      command.Parameters[1].Value = OptionName;
      command.Parameters[2].Value = ValueType;
      command.Parameters[3].Value = RealValue;
      command.Parameters[4].Value = StringValue;
    }
  }
}