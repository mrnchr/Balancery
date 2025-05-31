using ClosedXML.Excel;

namespace Mrnchr.Balancery.Statistics.Export
{
  public interface IExportProcessor
  {
    public void Process(XLWorkbook workbook);
  }
}