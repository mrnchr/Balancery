using System;
using System.IO;
using LinqToDB;

namespace Mrnchr.Balancery.Statistics
{
  public class BalanceryStatistics : IDisposable
  {
    private readonly IBalanceryStatisticsConfig _config;
    private readonly IDatabaseProvider _dbProvider;
    private readonly StatisticsCollector _collector;

    public IBalanceryStatisticsConfig Config => _config;
    public StatisticsCollector Collector => _collector;

    public BalanceryStatistics(IBalanceryStatisticsConfig config)
    {
      _config = config;
      
      if (!Directory.Exists(_config.DatabasePath))
        Directory.CreateDirectory(_config.DatabasePath);
      
      _dbProvider = new DatabaseProvider(new DataOptions()
        .UseSQLite($"Data Source = {Path.Combine(_config.DatabasePath, _config.DatabaseName)}; Foreign Keys = True"));
      
      _collector = new StatisticsCollector(_dbProvider);
    }

    public BalanceryStatistics(IBalanceryStatisticsConfig config, IDatabaseProvider dbProvider)
    {
      _config = config;
      _dbProvider = dbProvider;
      _collector = new StatisticsCollector(_dbProvider);
    }

    public void Dispose()
    {
      _dbProvider?.Dispose();
    }
  }
}