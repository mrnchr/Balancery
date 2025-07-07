using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Mrnchr.Balancery.Statistics.Benchmark
{
  public class Program
  {
    public static void Main(string[] args)
    {
      BenchmarkRunner.Run<ChangeCacheFieldAndInsertOrReplaceCallBenchmark>(
        ManualConfig.Create(DefaultConfig.Instance)
          .WithOptions(ConfigOptions.DisableOptimizationsValidator)
      );
    }
  }
}