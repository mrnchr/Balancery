using System;
using System.IO;
using Balancery.Statistics.Shared;
using Mrnchr.Balancery.Statistics.Database;
using Mrnchr.Balancery.Statistics.Utils;

namespace Mrnchr.Balancery.Statistics
{
  public class BStatistics : IDisposable
  {
    private const string DATA_PROVIDER_ASSEMBLY = "Balancery.Statistics.DataProvider";
    private const string SQLITE_PROVIDER_TYPE = "SQLiteProvider";

    private const string EXPORTER_ASSEMBLY = "Balancery.Statistics.Export";
    private const string XLSX_EXPORTER_TYPE = "XLSXExporter";

    private readonly IStatisticsConfig _config;
    private readonly BCollector _collector;

    public IStatisticsConfig Config => _config;
    public BCollector Collector => _collector;

    public DataProviderShell DataProviderShell { get; } = new DataProviderShell();

    public IDataProvider DataProvider
    {
      get => DataProviderShell.DataProvider;
      set
      {
        DisposeDataProvider();
        DataProviderShell.DataProvider = value;
      }
    }

    public ExporterShell ExporterShell { get; } = new ExporterShell();

    public IExporter Exporter
    {
      get => ExporterShell.Exporter;
      set => ExporterShell.Exporter = value;
    }

    public BStatistics(IStatisticsConfig config)
    {
      _config = config;

      if (!Directory.Exists(_config.DataFilePath))
        Directory.CreateDirectory(_config.DataFilePath);

      if (!_config.CustomDataProvider)
        CreateDefaultDataProvider();

      _collector = new BCollector(DataProviderShell);
      if (!_config.CustomExport)
        CreateDefaultExporter();
    }

    private void CreateDefaultDataProvider()
    {
      Type type = TypeUtils.GetType(SQLITE_PROVIDER_TYPE, DATA_PROVIDER_ASSEMBLY);
      if (type != null)
        DataProvider = (IDataProvider)Activator.CreateInstance(type, _config);
    }

    private void CreateDefaultExporter()
    {
      Type type = TypeUtils.GetType(XLSX_EXPORTER_TYPE, EXPORTER_ASSEMBLY);
      if (type != null)
        Exporter = (IExporter)Activator.CreateInstance(type, DataProviderShell);
    }

    public void Export()
    {
      Exporter.Export(_config.ExportFileTemplatePath, _config.ExportFilePath, _config.ExportFileName);
    }

    public void Dispose()
    {
      DisposeDataProvider();
    }

    private void DisposeDataProvider()
    {
      DataProviderShell.DataProvider?.Dispose();
      DataProviderShell.DataProvider = null;
    }
  }
}