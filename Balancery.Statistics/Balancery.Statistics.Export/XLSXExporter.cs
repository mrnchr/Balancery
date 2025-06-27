using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using ClosedXML.Excel;
using Mrnchr.Balancery.Statistics.Database;

namespace Mrnchr.Balancery.Statistics.Export
{
  public class XLSXExporter : IExporter
  {
    public const string SESSIONS_SHEET_NAME = "Sessions";
    public const string HORIZONTAL_TURNS_SHEET_NAME = "Horizontal Turns";
    public const string VERTICAL_TURNS_SHEET_NAME = "Vertical Turns";

    private readonly IDataProvider _dbProvider;
    
    private readonly List<IExportProcessor> _processors = new List<IExportProcessor>();

    public XLSXExporter(IDataProvider dbProvider)
    {
      _dbProvider = dbProvider;
    }

    public void Export(string templateFile, string outputPath, string outputFileName)
    {
      string destinationPath = Path.Combine(outputPath, outputFileName);
      if (File.Exists(templateFile))
      {
        File.Copy(templateFile, destinationPath);
      }

      Export(destinationPath);
    }

    public void Export(string outputFile)
    {
      XLWorkbook workbook = File.Exists(outputFile) ? new XLWorkbook(outputFile) : new XLWorkbook();
      DataTable sessions = _dbProvider.GetMetricsTable();
      CopyTableToWorksheet(workbook, SESSIONS_SHEET_NAME, sessions);

      DataTable turns = _dbProvider.GetTurnsTable();
      CopyTableToWorksheet(workbook, HORIZONTAL_TURNS_SHEET_NAME, turns);

      var exportData = new XLSXExportData { Workbook = workbook };
      foreach (IExportProcessor processor in _processors)
        processor.Process(exportData);

      workbook.SaveAs(outputFile);
    }

    private void CopyTableToWorksheet(XLWorkbook workbook, string name, DataTable table)
    {
      IXLWorksheet worksheet = GetOrCreateWorksheet(workbook, name);

      CopyTableToWorksheet(table, worksheet);
    }

    private void CopyTableToWorksheet(DataTable table, IXLWorksheet worksheet)
    {
      for (int i = 0; i < table.Columns.Count; i++)
      {
        worksheet.Cell(1, i + 1).Value = table.Columns[i].ColumnName;
      }

      for (int i = 0; i < table.Rows.Count; i++)
      {
        for (int j = 0; j < table.Columns.Count; j++)
        {
          worksheet.Cell(i + 2, j + 1).Value = Convert(table.Rows[i][j]);
        }
      }
    }

    private void CopyVerticalTurnsTableToWorksheet(DataTable table, IXLWorksheet worksheet)
    {
      for (int i = 0; i < table.Columns.Count; i++)
      {
        worksheet.Cell(1, i + 1).Value = table.Columns[i].ColumnName;
      }

      for (int i = 0; i < table.Rows.Count; i++)
      {
        for (int j = 0; j < table.Columns.Count; j++)
        {
          worksheet.Cell(i + 2, j + 1).Value = XLCellValue.FromObject(table.Rows[i][j]);
        }
      }
    }

    private static IXLWorksheet GetOrCreateWorksheet(XLWorkbook workbook, string name)
    {
      if (!workbook.Worksheets.TryGetWorksheet(name, out IXLWorksheet worksheet))
      {
        worksheet = workbook.AddWorksheet(name);
      }

      return worksheet;
    }

    private static XLCellValue Convert(object obj)
    {
      XLCellValue instance = XLCellValue.FromObject(obj);
      if (instance.TryConvert(out double number, CultureInfo.InvariantCulture))
        return number;

      return instance;
    }

    public void AddProcessor(IExportProcessor processor)
    {
      _processors.Add(processor);
    }

    public void RemoveProcessor(IExportProcessor processor)
    {
      _processors.Remove(processor);
    }
  }
}