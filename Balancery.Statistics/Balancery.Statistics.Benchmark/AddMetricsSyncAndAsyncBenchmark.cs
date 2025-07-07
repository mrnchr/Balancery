using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Mrnchr.Balancery.Statistics.Database;

namespace Mrnchr.Balancery.Statistics.Benchmark
{
  [MemoryDiagnoser]
  [IterationTime(100)]
  public class AddMetricsSyncAndAsyncBenchmark
  {
    private const int ITERATIONS = 1;
    private const int REPEAT = 1;
    private const int COUNT = REPEAT * ITERATIONS;

    private SQLiteProvider _dbProvider;
    private string _databasePath;
    private Task[] _tasks;
    private string[] _marks;

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

      _tasks = new Task[COUNT];
      for (int i = 0; i < COUNT; i++)
      {
        _tasks[i] = _dbProvider.RecordSessionMetricAsync(i, i.ToString(), i);
      }

      _marks = new string[COUNT];
      for (int i = 0; i < COUNT; i++)
        _marks[i] = i.ToString();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
      for (int i = 0; i < COUNT; i++)
      {
        _dbProvider.RemoveSession(i);
      }

      Array.Clear(_tasks);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
      _dbProvider.Dispose();
      if (!string.IsNullOrWhiteSpace(_databasePath))
        Directory.Delete(_databasePath, true);
    }

    [Benchmark(OperationsPerInvoke = ITERATIONS)]
    public void Add()
    {
      for (int i = 0; i < COUNT; i++)
      {
        _dbProvider.RecordActionValue(i, i, i, i);
      }
    }

    [Benchmark(OperationsPerInvoke = ITERATIONS)]
    public void AddExplicitAsync()
    {
      for (int i = 0; i < COUNT; i++)
      {
        _tasks[i] = _dbProvider.RecordActionValueAsync(i, i, i, i);
      }
    }
  }
}