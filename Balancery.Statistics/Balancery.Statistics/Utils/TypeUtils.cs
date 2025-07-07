using System;
using System.Linq;
using System.Reflection;

namespace Mrnchr.Balancery.Statistics.Utils
{
  public static class TypeUtils
  {
    private static Assembly[] GetAssemblies()
    {
      return AppDomain.CurrentDomain.GetAssemblies();
    }

    public static Type GetType(string typeName, string assemblyName)
    {
      Assembly assembly = GetAssemblies()
        .FirstOrDefault(x => x.GetName().Name == assemblyName);
      
      try
      {
        assembly ??= Assembly.Load(assemblyName);
      }
      catch
      {
        // ignored
      }

      return assembly
        ?.GetTypes()
        .FirstOrDefault(x => x.Name == typeName);
    }
  }
}