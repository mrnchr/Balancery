using ClosedXML.Excel;

namespace Mrnchr.Balancery.Statistics.Export
{
  public class XLSXExportData : IExportData
  {
    public XLWorkbook Workbook { get; set; }
  }
}