using UnityEngine;

namespace Mrnchr.Balancery.Runtime
{
  [CreateAssetMenu(menuName = CAC.PROJECT_MENU + "Academy Settings", fileName = "AcademySettings")]
  public class BAcademySettings : ScriptableObject
  {
    public BEnvironment EnvironmentPrefab;
    public int NumberOfSimulations;
    public int NumberOfEnvironments;
  }
}