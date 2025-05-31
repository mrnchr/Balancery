using Unity.MLAgents.Actuators;

namespace Mrnchr.Balancery.Runtime.Academy
{
  public interface IActionProvider
  {
    public void InsertActions(int sessionIndex, int turnIndex, ref ActionBuffers actionBuffers);
  }
}