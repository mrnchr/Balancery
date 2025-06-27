using System;
using System.IO;
using System.Reflection;
using Mrnchr.Balancery.Statistics.Database;
using Mrnchr.Balancery.Statistics.Export;

namespace Mrnchr.Balancery.Statistics
{
  public class BStatistics : IDisposable
  {
    private const string DATA_PROVIDER_ASSEMBLY = "Balancery.Statistics.DataProvider";
    private const string SQLITE_PROVIDER_TYPE = "SQLiteProvider";
    private readonly IStatisticsConfig _config;
    private readonly IDataProvider _dbProvider;
    private readonly BCollector _collector;
    private readonly XLSXExporter _exporter;

    public IStatisticsConfig Config => _config;
    public BCollector Collector => _collector;
    
    public IDataProvider DbProvider => _dbProvider;

    public BStatistics(IStatisticsConfig config)
    {
      _config = config;
      
      if (!Directory.Exists(_config.DatabasePath))
        Directory.CreateDirectory(_config.DatabasePath);

      Assembly assembly = Array.Find(AppDomain.CurrentDomain.GetAssemblies(), x => x.GetName().Name == DATA_PROVIDER_ASSEMBLY);
      if (assembly != null)
      {
        Type type = assembly.GetType(SQLITE_PROVIDER_TYPE);
        if (type != null)
        {
          _dbProvider = (IDataProvider)Activator.CreateInstance(type, _config);
        }
      }
      
      _collector = new BCollector(_dbProvider);
      _exporter = new XLSXExporter(_dbProvider);
    }

    public BStatistics(IStatisticsConfig config, IDataProvider dbProvider)
    {
      _config = config;
      _dbProvider = dbProvider;
      _collector = new BCollector(_dbProvider);
      _exporter = new XLSXExporter(_dbProvider);
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