using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Mrnchr.Balancery.Editor.Setup
{
  public static class DefineSetup
  {
    private const string PROJECT_DEFINE = "BALANCERY";
    
    [MenuItem(MIC.PROJECT_TOOLS_MENU + "Enable Balancery")]
    public static void EnableBalanceryDefine()
    {
      AddProjectDefine(PROJECT_DEFINE);
    }

    [MenuItem(MIC.PROJECT_TOOLS_MENU + "Disable Balancery")]
    public static void DisableBalanceryDefine()
    {
      RemoveProjectDefine(PROJECT_DEFINE);
    }

    private static void AddProjectDefine(string id)
    {
      bool added = false;
      int totGroupsModified = 0;
      NamedBuildTarget[] targets = GetNamedBuildTargets();
      foreach (NamedBuildTarget target in targets)
      {
        string defines = PlayerSettings.GetScriptingDefineSymbols(target);
        string[] singleDefines = defines.Split(';');
        if (Array.IndexOf(singleDefines, id) != -1)
          continue;
        
        added = true;
        totGroupsModified++;
        defines += defines.Length > 0 ? ";" + id : id;
        PlayerSettings.SetScriptingDefineSymbols(target, defines);
      }

      if (added)
        Debug.Log($"Balancery : added global define \"{id}\" to {totGroupsModified} BuildTargetGroups");
    }

    private static void RemoveProjectDefine(string id)
    {
      bool removed = false;
      int totGroupsModified = 0;
      NamedBuildTarget[] targetGroups = GetNamedBuildTargets();
      foreach (NamedBuildTarget target in targetGroups)
      {
        string defines = PlayerSettings.GetScriptingDefineSymbols(target);
        string[] singleDefines = defines.Split(';');
        if (Array.IndexOf(singleDefines, id) == -1) 
          continue;
        
        removed = true;
        totGroupsModified++;
        string rmDefines = defines.Replace(singleDefines.Length > 1 ? id + ";" : id, "");
        PlayerSettings.SetScriptingDefineSymbols(target, rmDefines);
      }
      
      if (removed)
        Debug.Log($"Balancery : removed global define \"{id}\" from {totGroupsModified} BuildTargetGroups");
    }

    private static NamedBuildTarget[] GetNamedBuildTargets()
    {
      NamedBuildTarget[] targets = Enum.GetValues(typeof(BuildTarget))
        .Cast<BuildTarget>()
        .Select(x => (Group: BuildPipeline.GetBuildTargetGroup(x), Target: x))
        .Where(x => BuildPipeline.IsBuildTargetSupported(x.Group, x.Target))
        .Select(x => x.Group)
        .Distinct()
        .Select(NamedBuildTarget.FromBuildTargetGroup)
        .ToArray();
      return targets;
    }
  }
}