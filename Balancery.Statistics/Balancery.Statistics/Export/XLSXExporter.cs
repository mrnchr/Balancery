using System.Data;
using System.IO;
using ClosedXML.Excel;
using Mrnchr.Balancery.Statistics.Database;

namespace Mrnchr.Balancery.Statistics.Export
{
  public class XLSXExporter : IExporter
  {
    public const string SESSION_SHEET_NAME = "Sessions";
    public const string TURN_SHEET_NAME = "Turns";

    private readonly IDatabaseProvider _dbProvider;

    public XLSXExporter(IDatabaseProvider dbProvider)
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
      CopyTableToWorksheet(workbook, SESSION_SHEET_NAME, sessions);
      
      DataTable turns = _dbProvider.GetTurnsTable();
      CopyTableToWorksheet(workbook, TURN_SHEET_NAME, turns);

      workbook.SaveAs(outputFile);
    }

    private void CopyTableToWorksheet(XLWorkbook workbook, string name, DataTable table)
    {
      if (!workbook.Worksheets.TryGetWorksheet(name, out IXLWorksheet worksheet))
      {
        worksheet = workbook.AddWorksheet(name);
      }

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
          worksheet.Cell(i + 2, j + 1).Value = XLCellValue.FromObject(table.Rows[i][j]);
        }
      }
    }
  }
}