namespace Mrnchr.Balancery.Statistics
{
  public interface IStatisticsConfig
  {
    public bool CustomDataProvider { get; set; }
    public string DataFilePath { get; set; }
    public string DataFileName { get; set; }
    
    public bool CustomExport { get; set; }
    public string ExportFileTemplatePath { get; set; }
    public string ExportFilePath { get; set; }
    public string ExportFileName { get; set; }
  }
}