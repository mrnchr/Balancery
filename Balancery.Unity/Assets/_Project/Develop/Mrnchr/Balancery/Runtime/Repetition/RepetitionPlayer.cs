using System;

namespace Mrnchr.Balancery.Runtime.Repetition
{
  public static class RepetitionPlayer
  {
    public static string DatabaseFile;
    public static bool IsRepetition;
    public static int SessionIndex;
    public static float SimulationSpeed;

    public static event Action OnPause;
    public static event Action OnResume;
    public static event Action OnNextTurn;
    public static event Action OnPreviousTurn;
    
    public static event Action OnRepeat;

    public static void Pause()
    {
      OnPause?.Invoke();
    }

    public static void Resume()
    {
      OnResume?.Invoke();
    }

    public static void NextTurn()
    {
      OnNextTurn?.Invoke();
    }

    public static void PreviousTurn()
    {
      OnPreviousTurn?.Invoke();
    }

    public static void Repeat()
    {
      OnRepeat?.Invoke();
    }
  }
}