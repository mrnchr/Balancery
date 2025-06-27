using System;
using System.IO;
using LinqToDB;
using LinqToDB.Data;
using Mrnchr.Balancery.Statistics.Database;
using Mrnchr.Balancery.Statistics.Export;
using NUnit.Framework;

namespace Mrnchr.Balancery.Statistics.Testing
{
  public class Tests
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void WhenCreateBalanceryStatisticsThenDatabaseShouldBeConnected()
    {
      TurnMetricData turnMetricData = new TurnMetricData();
      XLSXExportData xlsxExportData = new XLSXExportData();
      string dataFilePath = "Database";
      string dataFileName = "database.db";
      Console.WriteLine($"{Path.GetFullPath(Path.Combine(dataFilePath, dataFileName))}");
      BStatistics statistics = null;

      if (!string.IsNullOrWhiteSpace(dataFilePath) && !Directory.Exists(dataFilePath))
        Directory.CreateDirectory(dataFilePath);
      
      // Assert.
      Assert.DoesNotThrow(() => statistics =
        new BStatistics(new StatisticsConfig { DataFilePath = dataFilePath, DataFileName = dataFileName }));
      Assert.IsNotNull(statistics.DataProvider);
      Assert.IsNotNull(statistics.Exporter);
      
      statistics.Dispose();
      if (!string.IsNullOrWhiteSpace(dataFilePath))
        Directory.Delete(dataFilePath, true);
    }

    [Test]
    public void WhenCreateDatabaseConnectionAndHasNotDatabaseDirectoryAndFileThenDatabaseShouldBeCreated()
    {
      // Arrange.
      var databasePath = "Database";
      var databaseName = "database";
      string combinePath = Path.Combine(databasePath, databaseName);

      if (!Directory.Exists(databasePath))
        Directory.CreateDirectory(databasePath);

      // Act.
      var dbConnection = new StatisticsDatabaseConnection(new DataOptions()
        .UseSQLite($"Data Source = {combinePath}; Foreign Keys = True"));

      // Assert.
      Console.WriteLine($"{Path.GetFullPath(combinePath)}");
      Assert.DoesNotThrow(() => dbConnection.Execute("SELECT 1"));

      dbConnection.Dispose();
      if (!string.IsNullOrWhiteSpace(databasePath))
        Directory.Delete(databasePath, true);
    }

    [Test]
    public void WhenCreateDatabaseConnectionAndPathContainsDifferentSlashesThenDatabaseShouldBeCreated()
    {
      // Arrange.
      var databasePath = "Root/Branch\\Leaf";
      var databaseName = "database";
      string combinePath = Path.Combine(databasePath, databaseName);

      if (!Directory.Exists(databasePath))
        Directory.CreateDirectory(databasePath);

      Console.WriteLine($"{combinePath}");

      // Act.
      var dbConnection = new StatisticsDatabaseConnection(new DataOptions()
        .UseSQLite($"Data Source = {combinePath}; Foreign Keys = True"));
      // Assert.
      Console.WriteLine($"{Path.GetFullPath(combinePath)}");
      Assert.DoesNotThrow(() => dbConnection.Execute("SELECT 1"));

      dbConnection.Dispose();
      if (!string.IsNullOrWhiteSpace(databasePath))
        Directory.Delete(databasePath, true);
    }

    [Test]
    public void WhenAddMetricValueAndTableIsEmptyThenValueShouldBeInserted()
    {
      // Arrange.
      var databasePath = "";
      var databaseName = "database.db";
      string combinePath = Path.Combine(databasePath, databaseName);
      Console.WriteLine($"{combinePath}");
      IStatisticsConfig config = new StatisticsConfig
      {
        DataFilePath = databasePath,
        DataFileName = databaseName
      };

      if (!string.IsNullOrWhiteSpace(databasePath) && !Directory.Exists(databasePath))
        Directory.CreateDirectory(databasePath);

      var dbProvider = new SQLiteProvider(config);

      // Act.

      // Assert.
      Assert.DoesNotThrow(() => dbProvider.RecordSessionMetric(1, "key", 1));
      Assert.DoesNotThrow(() => dbProvider.RecordSessionMetric(1, "key2", 1));
      Assert.DoesNotThrow(() => dbProvider.RecordActionValue(2, 3, 2, 1));

      dbProvider.Dispose();
      if (!string.IsNullOrWhiteSpace(databasePath))
        Directory.Delete(databasePath, true);
    }
  }
}