using System.Collections.Generic;
using System.Data;
using System.Linq;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics.Filters;
using Debug = UnityEngine.Debug;

namespace Mrnchr.Balancery.Runtime.Statistics
{
  public class StatisticsAnalyzer
  {
    private readonly DataTable _dataTable;
    private List<Dictionary<string, float>> _matchData;

    public StatisticsAnalyzer(List<Dictionary<string, float>> matchData)
    {
      _dataTable = ConvertToDataTable(matchData);
    }

    public void GenerateClusterStatistics(int numberOfClusters)
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

      var clusterStats = new Dictionary<int, Dictionary<string, float>>();
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
          entry[column.ColumnName] += (float) _dataTable.Rows[i][column.ColumnName];
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