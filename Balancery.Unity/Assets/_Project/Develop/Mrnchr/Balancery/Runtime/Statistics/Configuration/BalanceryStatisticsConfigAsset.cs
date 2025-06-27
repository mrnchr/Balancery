using System;
using System.Globalization;
using System.IO;
using Mrnchr.Balancery.Statistics;
using UnityEngine;

namespace Mrnchr.Balancery.Runtime.Statistics.Configuration
{
  [CreateAssetMenu(menuName = CAC.PROJECT_MENU + "StatisticsConfig", fileName = "BalanceryStatisticsConfig")]
  public class BalanceryStatisticsConfigAsset : ScriptableObject
  {
    public string DatabasePath = Path.Combine(Application.dataPath, "../Balancery");
    public bool UseTemplate;
    public string ExportFileTemplatePath = Application.dataPath;
    public string ExportFilePath = Application.dataPath;

#if BALANCERY_STATISTICS
    public IStatisticsConfig CreateConfig()
    {
      return CreateConfig(this);
    }

    public static IStatisticsConfig CreateConfig(BalanceryStatisticsConfigAsset configAsset)
    {
      var time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
      return new StatisticsConfig
      {
        DatabasePath = configAsset.DatabasePath,
        DatabaseName = $"{time}.db",
        ExportFileTemplatePath = configAsset.UseTemplate ? configAsset.ExportFileTemplatePath : "",
        ExportFilePath = configAsset.ExportFilePath,
        ExportFileName = $"{time}.xlsx"
      };
    }
#endif
  }
}