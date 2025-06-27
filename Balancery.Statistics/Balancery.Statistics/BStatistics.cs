using System;
using System.IO;
using Mrnchr.Balancery.Statistics.Database;
using Mrnchr.Balancery.Statistics.Utils;

namespace Mrnchr.Balancery.Statistics
{
  public class BStatistics : IDisposable
  {
    private const string DATA_PROVIDER_ASSEMBLY = "Balancery.Statistics.DataProvider";
    private const string SQLITE_PROVIDER_TYPE = "SQLiteProvider";
    
    private const string EXPORTER_ASSEMBLY = "Balancery.Statistics.Export";
    private const string XLSX_EXPORTER_TYPE = "XLSXExporter";
    
    private readonly IStatisticsConfig _config;
    private readonly BCollector _collector;
    
    private IDataProvider _dbProvider;
    private IExporter _exporter;

    public IStatisticsConfig Config => _config;
    public BCollector Collector => _collector;

    public IDataProvider DbProvider => _dbProvider;

    public BStatistics(IStatisticsConfig config)
    {
      _config = config;

      if (!Directory.Exists(_config.DatabasePath))
        Directory.CreateDirectory(_config.DatabasePath);

      CreateDefaultDataProvider();

      _collector = new BCollector(_dbProvider);
      CreateDefaultExporter();
    }

    private void CreateDefaultDataProvider()
    {
      Type type = TypeUtils.GetType(SQLITE_PROVIDER_TYPE, DATA_PROVIDER_ASSEMBLY);
      if (type != null)
      {
        _dbProvider = (IDataProvider)Activator.CreateInstance(type, _config);
      }
    }

    private void CreateDefaultExporter()
    {
      Type type = TypeUtils.GetType(XLSX_EXPORTER_TYPE, EXPORTER_ASSEMBLY);
      if (type != null)
      {
        _exporter = (IExporter)Activator.CreateInstance(type, _dbProvider);
      }
    }

    public void Export()
    {
      _exporter.Export(_config.ExportFileTemplatePath, _config.ExportFilePath, _config.ExportFileName);
    }

    public void Dispose()
    {
      _dbProvider?.Dispose();
    }
  }
}