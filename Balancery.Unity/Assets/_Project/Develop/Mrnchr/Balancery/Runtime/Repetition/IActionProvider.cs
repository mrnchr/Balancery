using Unity.MLAgents.Actuators;

namespace Mrnchr.Balancery.Runtime.Repetition
{
  public interface IActionProvider
  {
    public void InsertActions(int sessionIndex, int turnIndex, ref ActionBuffers actionBuffers);
  }
}