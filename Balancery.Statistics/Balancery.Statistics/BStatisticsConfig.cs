namespace Mrnchr.Balancery.Statistics
{
  public class BalanceryStatisticsConfig : IBalanceryStatisticsConfig
  {
    public string DatabasePath { get; set; }
    public string DatabaseName { get; set; }
    public string ExportFileTemplatePath { get; set; }
    public string ExportFilePath { get; set; }
    public string ExportFileName { get; set; }
  }
}