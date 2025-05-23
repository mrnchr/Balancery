using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace Mrnchr.Balancery.Runtime.Statistics.Configuration
{
  [CreateAssetMenu(menuName = CAC.PROJECT_MENU + "Metrics Config", fileName = "MetricsConfig")]
  public class MetricsConfig : ScriptableObject
  {
    [ListDrawerSettings(ShowElementLabels = true)]
    public List<MetricData> Metrics = new List<MetricData>();
  }
}