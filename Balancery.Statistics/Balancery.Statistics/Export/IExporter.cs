using Mrnchr.Balancery.Statistics.Export;

namespace Mrnchr.Balancery.Statistics
{
  public interface IExporter
  {
    void Export(string templateFile, string outputPath, string outputFileName);
    void Export(string outputFile);
    
    void AddProcessor(IExportProcessor processor);
    void RemoveProcessor(IExportProcessor processor);
  }
}