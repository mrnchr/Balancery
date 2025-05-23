using System;
using System.IO;
using LinqToDB;
using Mrnchr.Balancery.Statistics.Database;
using Mrnchr.Balancery.Statistics.Export;

namespace Mrnchr.Balancery.Statistics
{
  public class BalanceryStatistics : IDisposable
  {
    private readonly IBalanceryStatisticsConfig _config;
    private readonly IDatabaseProvider _dbProvider;
    private readonly StatisticsCollector _collector;
    private readonly XLSXExporter _exporter;

    public IBalanceryStatisticsConfig Config => _config;
    public StatisticsCollector Collector => _collector;
    
    public IDatabaseProvider DbProvider => _dbProvider;

    public BalanceryStatistics(IBalanceryStatisticsConfig config)
    {
      _config = config;
      
      if (!Directory.Exists(_config.DatabasePath))
        Directory.CreateDirectory(_config.DatabasePath);
      
      _dbProvider = new DatabaseProvider(new DataOptions()
        .UseSQLite($"Data Source = {Path.Combine(_config.DatabasePath, _config.DatabaseName)}; Foreign Keys = True"));
      
      _collector = new StatisticsCollector(_dbProvider);
      _exporter = new XLSXExporter(_dbProvider);
    }

    public BalanceryStatistics(IBalanceryStatisticsConfig config, IDatabaseProvider dbProvider)
    {
      _config = config;
      _dbProvider = dbProvider;
      _collector = new StatisticsCollector(_dbProvider);
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