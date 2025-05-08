using System;

namespace Mrnchr.Balancery.Runtime.Statistics.Configuration
{
  [Serializable]
  public class MetricData
  {
    public string Id;
    public string Name;
    public string DisplayName;
    public bool Clustered;
  }
}