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
    public bool CustomDataProvider;
    public string DatabasePath = Path.Combine(Application.dataPath, "../Balancery");
    public bool CustomExport;
    public bool UseTemplate;
    public string ExportFileTemplatePath = Application.dataPath;
    public string ExportFilePath = Application.dataPath;

#if BALANCERY_STATISTICS
    public IStatisticsConfig CreateConfig()
    {
      var time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
      return new StatisticsConfig
      {
        CustomDataProvider = CustomDataProvider,
        DataFilePath = DatabasePath,
        DataFileName = $"{time}.db",
        CustomExport = CustomExport,
        ExportFileTemplatePath = UseTemplate ? ExportFileTemplatePath : "",
        ExportFilePath = ExportFilePath,
        ExportFileName = $"{time}.xlsx"
      };
    }
#endif
  }
}