using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using LinqToDB;
using Mrnchr.Balancery.Statistics.Database;

namespace Mrnchr.Balancery.Statistics.Benchmark
{
  [MemoryDiagnoser]
  [IterationTime(100)]
  public class ChangeCacheFieldAndInsertOrReplaceCallBenchmark
  {
    private const int COUNT = 1;

    private readonly SessionMetricData _cacheSessionMetric = new SessionMetricData();

    private SQLiteProvider _dbProvider;
    private string _databasePath;
    private string[] _marks;
    private SessionMetricData[] _metrics;

    [GlobalSetup]
    public void Setup()
    {
      _databasePath = "";
      var databaseName = "database.db";
      string combinePath = Path.Combine(_databasePath, databaseName);
      Console.WriteLine($"Database path: {Path.GetFullPath(combinePath)}");
      IStatisticsConfig config = new StatisticsConfig
      {
        DataFilePath = _databasePath,
        DataFileName = databaseName
      };
      
      if (!string.IsNullOrWhiteSpace(_databasePath) && !Directory.Exists(_databasePath))
        Directory.CreateDirectory(_databasePath);

      _dbProvider = new SQLiteProvider(config);

      _marks = new string[COUNT];
      for (int i = 0; i < COUNT; i++)
        _marks[i] = i.ToString();

      _metrics = new SessionMetricData[COUNT];
      for (int i = 0; i < COUNT; i++)
      {
        _metrics[i] = new SessionMetricData
        {
          SessionNumber = i,
          MetricName = _marks[i]
        };
        ((IData)_metrics[i]).SetValue(i);
      }
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
      for (int i = 0; i < COUNT; i++)
      {
        _dbProvider.RemoveSession(i);
      }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
      _dbProvider.Dispose();
      if (!string.IsNullOrWhiteSpace(_databasePath))
        Directory.Delete(_databasePath, true);
    }

    // [Benchmark(OperationsPerInvoke = COUNT)]
    public void ChangeCacheField()
    {
      for (int i = 0; i < COUNT; i++)
      {
        ChangeSessionMetric(i, _marks[i], i);
      }
    }

    // [Benchmark(OperationsPerInvoke = COUNT)]
    public void ChangeCacheFieldWithoutValue()
    {
      for (int i = 0; i < COUNT; i++)
      {
        ChangeSessionMetricWithoutValue(i, _marks[i]);
      }
    }

    // [Benchmark(OperationsPerInvoke = COUNT)]
    public void InsertOrReplaceCall()
    {
      for (int i = 0; i < COUNT; i++)
      {
        _dbProvider.Connection.InsertOrReplace(_metrics[i]);
      }
    }
    
    [Benchmark(OperationsPerInvoke = COUNT)]
    public void ReportAction()
    {
      for (int i = 0; i < COUNT; i++)
      {
        _dbProvider.Connection.RecordSession(_metrics[i]);
      }
    }

    private void ChangeSessionMetric<TType>(int sessionNumber, string metricName, TType value)
    {
      _cacheSessionMetric.SessionNumber = sessionNumber;
      _cacheSessionMetric.MetricName = metricName;
      ((IData)_cacheSessionMetric).SetValue(value);
    }

    private void ChangeSessionMetricWithoutValue(int sessionNumber, string metricName)
    {
      _cacheSessionMetric.SessionNumber = sessionNumber;
      _cacheSessionMetric.MetricName = metricName;
    }
  }
}