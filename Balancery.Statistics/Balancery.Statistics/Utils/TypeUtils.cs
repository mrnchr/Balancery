using System;
using System.Reflection;

namespace Mrnchr.Balancery.Statistics.Utils
{
  public static class TypeUtils
  {
    private static Assembly[] _assemblies;

    public static Assembly[] GetAssemblies()
    {
      return _assemblies ??= AppDomain.CurrentDomain.GetAssemblies();
    }

    public static Type GetType(string typeName, string assemblyName)
    {
      return Array.Find(GetAssemblies(), x => x.GetName().Name == assemblyName)?.GetType(typeName);
    }
  }
}