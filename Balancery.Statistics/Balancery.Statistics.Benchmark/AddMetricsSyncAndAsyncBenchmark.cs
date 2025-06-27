using System;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using LinqToDB;
using Mrnchr.Balancery.Statistics.Database;

namespace Mrnchr.Balancery.Statistics.Benchmark
{
  [MemoryDiagnoser]
  public class AddMetricsSyncAndAsyncBenchmark
  {
    private const int COUNT = 100;

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
      if (!string.IsNullOrWhiteSpace(_databasePath) && !Directory.Exists(_databasePath))
        Directory.CreateDirectory(_databasePath);

      _dbProvider = new SQLiteProvider(new DataOptions()
        .UseSQLite($"Data Source = {combinePath}"));

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
    }

    [GlobalCleanup]
    public void Cleanup()
    {
      _dbProvider.Dispose();
      if (!string.IsNullOrWhiteSpace(_databasePath))
        Directory.Delete(_databasePath, true);
    }

    // [Benchmark(OperationsPerInvoke = COUNT)]
    public void Add()
    {
      for (int i = 0; i < COUNT; i++)
      {
        _dbProvider.RecordSessionMetric(i, _marks[i], i);
      }
    }

    [Benchmark(OperationsPerInvoke = COUNT)]
    public void AddExplicitAsync()
    {
      for (int i = 0; i < COUNT; i++)
      {
        _ = _dbProvider.RecordSessionMetricAsync(i, _marks[i], i);
      }
    }

    // [Benchmark(OperationsPerInvoke = COUNT)]
    public void AddImplicitAsync()
    {
      Task.WaitAll(_tasks);
    }
  }
}