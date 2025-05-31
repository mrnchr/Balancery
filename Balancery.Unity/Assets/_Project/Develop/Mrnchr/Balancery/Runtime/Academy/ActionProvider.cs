using System.Collections.Generic;
using Mrnchr.Balancery.Runtime.Statistics;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Mrnchr.Balancery.Runtime.Academy
{
  public class ActionProvider : IActionProvider
  {
    private readonly StatisticsBridge _statistics;

    public ActionProvider(StatisticsBridge statistics)
    {
      _statistics = statistics;
    }

    public void InsertActions(int sessionIndex, int turnIndex, ref ActionBuffers actionBuffers)
    {
      List<float> list = _statistics.GetActions(sessionIndex, turnIndex);
      for (int i = 0; i < list.Count; i++)
      {
        if (i < actionBuffers.ContinuousActions.Length)
          actionBuffers.ContinuousActions.Array[i] = list[i];
        else
          actionBuffers.DiscreteActions.Array[i] = Mathf.RoundToInt(list[i - actionBuffers.ContinuousActions.Length]);
      }
    }
  }
}