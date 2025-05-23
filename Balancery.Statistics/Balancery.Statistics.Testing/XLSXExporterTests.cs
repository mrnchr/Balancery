using System;
using System.IO;
using ClosedXML.Excel;
using LinqToDB;
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
      var dbProvider = new DatabaseProvider(new DataOptions()
        .UseSQLite($"Data Source = {databaseName}; Foreign Keys = True"));
      var exporter = new XLSXExporter(dbProvider);
      dbProvider.RecordMetricValue(1, "Test", 10);

      // Act.
      exporter.Export(tableFile);

      // Assert.
      var workbook = new XLWorkbook(tableFile);
      var worksheet = workbook.Worksheet(XLSXExporter.SESSION_SHEET_NAME);
      Console.WriteLine($"{worksheet.Cell("B2").Value}");
      Assert.IsTrue(worksheet.Cell("B2").Value.ToString() == "10");

      dbProvider.Dispose();
      File.Delete(databaseName);
      File.Delete(tableFile);
    }
  }
}