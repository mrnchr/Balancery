using System.IO;
using UnityEngine;

namespace Mrnchr.Balancery.Runtime.Statistics.Configuration
{
  [CreateAssetMenu(menuName = CAC.PROJECT_MENU + "StatisticsConfig", fileName = "BalanceryStatisticsConfig")]
  public class BalanceryStatisticsConfigAsset : ScriptableObject
  {
    public string DatabasePath = Path.Combine(Application.dataPath, "../Balancery");
  }
}