using System.Collections.Generic;
using System.Data;
using System.Linq;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics.Filters;
using Mrnchr.Balancery.Runtime.Statistics.Configuration;
using UnityEngine;

namespace Mrnchr.Balancery.Runtime.Statistics
{
  public class StatisticsAnalyzer
  {
    private readonly Dictionary<string, float> _averages = new Dictionary<string, float>();
    private readonly Dictionary<string, List<float>> _verticalData = new Dictionary<string, List<float>>();
    private readonly Dictionary<string, Dictionary<float, float>> _ratios = new Dictionary<string, Dictionary<float, float>>();
    private readonly Dictionary<string, MetricData> _metrics;
    private readonly List<Dictionary<string, float>> _horizontalData;
    private readonly DataTable _dataTable;

    public StatisticsAnalyzer(List<MetricData> userMetrics, List<Dictionary<string, float>> horizontalData)
    {
      _horizontalData = horizontalData;
      foreach (Dictionary<string, float> entry in horizontalData)
      {
        foreach (KeyValuePair<string, float> pair in entry)
        {
          if (!_verticalData.TryAdd(pair.Key, new List<float>()))
            _verticalData[pair.Key].Add(pair.Value);
        }
      }

      _dataTable = ConvertToDataTable(horizontalData);
      _metrics = userMetrics.ToDictionary(x => x.Id, x => x);
    }

    public void GenerateStatistics(int numberOfClusters)
    {
      CalculateAverage();

      _ratios.Clear();
      foreach (KeyValuePair<string, List<float>> pair in _verticalData)
      {
        if(!_metrics.TryGetValue(pair.Key, out MetricData metric) || !metric.CalculateRatio) 
          continue;
        
        _ratios.TryAdd(pair.Key, new Dictionary<float, float>());
        float unitRatio = 1f / pair.Value.Count;
        foreach (float value in pair.Value)
        {
          if (!_ratios[pair.Key].TryAdd(value, unitRatio))
            _ratios[pair.Key][value] += unitRatio;
        }
      }

      KMeansClusterCollection clusters = GenerateClusters(numberOfClusters, out Dictionary<int, Dictionary<string, float>> clusterStats);

      Debug.Log($"Средние значения матчей:");
      foreach (KeyValuePair<string, float> average in _averages)
      {
        Debug.Log($"  average_{average.Key}: {average.Value:F2}");
      }
      
      Debug.Log($"Доли значений матчей:");
      foreach (KeyValuePair<string, Dictionary<float, float>> ratio in _ratios)
      {
        Debug.Log($"  ratio_{ratio.Key}:");
        foreach (KeyValuePair<float, float> entry in ratio.Value)
        {
          Debug.Log($"    {entry.Key:F2}: {entry.Value:F2}");
        }
      }

      Debug.Log($"Кластеризация завершена. Найдено {clusters.Count} кластеров:");
      foreach (KeyValuePair<int, Dictionary<string, float>> cluster in clusterStats)
      {
        Debug.Log($"Кластер {cluster.Key}:");
        Debug.Log($"  Количество матчей: {cluster.Value["Count"]}");
        foreach (DataColumn column in _dataTable.Columns)
        {
          Debug.Log($"  {column.ColumnName}: {cluster.Value[column.ColumnName]:F2}");
        }
      }
    }

    private KMeansClusterCollection GenerateClusters(int numberOfClusters, out Dictionary<int, Dictionary<string, float>> clusterStats)
    {
      var codebook = new Codification(_dataTable);
      DataTable normalizedData = codebook.Apply(_dataTable);

      double[][] data = normalizedData.ToJagged<double>();

      var kmeans = new KMeans(numberOfClusters)
      {
        MaxIterations = 100,
        Tolerance = 1e-5f
      };

      KMeansClusterCollection clusters = kmeans.Learn(data);

      int[] labels = clusters.Decide(data);

      clusterStats = new Dictionary<int, Dictionary<string, float>>();
      for (int i = 0; i < data.Length; i++)
      {
        int clusterId = labels[i];
        if (!clusterStats.TryGetValue(clusterId, out Dictionary<string, float> entry))
        {
          entry = new Dictionary<string, float>();
          clusterStats[clusterId] = entry;
          foreach (DataColumn column in _dataTable.Columns)
          {
            entry[column.ColumnName] = 0f;
          }

          entry["Count"] = 0f;
        }

        foreach (DataColumn column in _dataTable.Columns)
        {
          entry[column.ColumnName] += (float)_dataTable.Rows[i][column.ColumnName];
        }

        entry["Count"] += 1;
      }

      foreach (int cluster in clusterStats.Keys.ToList())
      {
        float count = clusterStats[cluster]["Count"];
        foreach (DataColumn column in _dataTable.Columns)
        {
          clusterStats[cluster][column.ColumnName] /= count;
        }
      }

      return clusters;
    }

    private void CalculateAverage()
    {
      _averages.Clear();
      foreach (KeyValuePair<string, List<float>> pair in _verticalData)
      {
        if (!_metrics.TryGetValue(pair.Key, out MetricData metric) || !metric.CalculateAverage)
          continue;

        _averages.Add(pair.Key, pair.Value.Average());
      }
    }

    private DataTable ConvertToDataTable(List<Dictionary<string, float>> rawData)
    {
      var table = new DataTable();

      foreach (string key in rawData[0].Keys)
      {
        table.Columns.Add(key, typeof(float));
      }

      foreach (Dictionary<string, float> entry in rawData)
      {
        DataRow row = table.NewRow();
        foreach (KeyValuePair<string, float> kvp in entry)
        {
          row[kvp.Key] = kvp.Value;
        }

        table.Rows.Add(row);
      }

      return table;
    }
  }
}