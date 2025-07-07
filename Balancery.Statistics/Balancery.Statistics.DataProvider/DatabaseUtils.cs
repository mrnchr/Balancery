using System.Data.Common;

namespace Mrnchr.Balancery.Statistics.Database
{
  public static class DatabaseUtils
  {
    public static DbParameter CreateParameter(this DbCommand obj, string name)
    {
      DbParameter instance = obj.CreateParameter();
      instance.ParameterName = name;
      return instance;
    }
  }
}