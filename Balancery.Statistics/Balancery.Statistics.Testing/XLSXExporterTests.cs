using System;
using System.IO;
using Balancery.Statistics.Shared;
using ClosedXML.Excel;
using Mrnchr.Balancery.Statistics.Database;
using Mrnchr.Balancery.Statistics.Export;
using NUnit.Framework;

namespace Mrnchr.Balancery.Statistics.Testing
{
  public class XLSXExporterTests
  {
    [Test]
    public void WhenExportAndValuesHasInDatabaseThenValuesHasInTable()
    {
      // Arrange.
      string databaseName = "database.db";
      var tableFile = "table.xlsx";
      IStatisticsConfig config = new StatisticsConfig
      {
        DatabasePath = "",
        DatabaseName = databaseName
      };
      var dataProvider = new SQLiteProvider(config);
      var exporter = new XLSXExporter(new DataProviderShell { DataProvider = dataProvider });
      dataProvider.RecordSessionMetric(1, "Test", 10);

      // Act.
      exporter.Export(tableFile);

      // Assert.
      var workbook = new XLWorkbook(tableFile);
      var worksheet = workbook.Worksheet(XLSXExporter.SESSIONS_SHEET_NAME);
      Console.WriteLine($"{worksheet.Cell("B2").Value}");
      Assert.IsTrue(worksheet.Cell("B2").Value.ToString() == "10");

      dataProvider.Dispose();
      File.Delete(databaseName);
      File.Delete(tableFile);
    }
  }
}