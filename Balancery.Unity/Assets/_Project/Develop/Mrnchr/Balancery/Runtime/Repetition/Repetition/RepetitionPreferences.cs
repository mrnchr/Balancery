using System;
using Mrnchr.Balancery.Runtime.Repetition;
using TriInspector;
using UnityEditor;
using UnityEngine;

namespace Mrnchr.Balancery.Editor
{
  [FilePath("UserSettings/Balancery/RepetitionPreferences.asset", FilePathAttribute.Location.ProjectFolder)]
  public class RepetitionPreferences : ScriptableSingleton<RepetitionPreferences>
  {
    public bool EnableRepetition;

    [EnableIf(nameof(EnableRepetition))]
    public string DatabaseFile; 
    
    [EnableIf(nameof(EnableRepetition))]
    public int SessionIndex;

    [EnableIf(nameof(EnableRepetition))]
    [Range(0f, 10f)]
    public float SimulationSpeed;

    [NonSerialized]
    private bool _isPaused;

    [Button(ButtonSizes.Medium, "\u23f8")]
    [EnableIf(nameof(EnableRepetition))]
    [DisableIf(nameof(_isPaused))]
    public void PauseRepetition()
    {
      _isPaused = true;
      RepetitionPlayer.Pause();
    }

    [Button(ButtonSizes.Medium, "\u25b6")]
    [EnableIf(nameof(EnableRepetition))]
    [EnableIf(nameof(_isPaused))]
    public void ResumeRepetition()
    {
      _isPaused = false;
      RepetitionPlayer.Resume();
    }

    [Button(ButtonSizes.Medium, "\u23e9")]
    [EnableIf(nameof(EnableRepetition))]
    public void NextTurn()
    {
      RepetitionPlayer.NextTurn();
    }

    [Button(ButtonSizes.Medium, "Repeat")]
    [EnableIf(nameof(EnableRepetition))]
    public void Repeat()
    {
      RepetitionPlayer.Repeat();
    }

    // [Button("<-")]
    // [EnableIf(nameof(EnableRepetition))]
    // public void PreviousTurn()
    // {
    //   RepetitionPlayer.PreviousTurn();
    // }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
      RepetitionPlayer.DatabaseFile = instance.DatabaseFile;
      RepetitionPlayer.IsRepetition = instance.EnableRepetition;
      RepetitionPlayer.SessionIndex = instance.SessionIndex;
      RepetitionPlayer.SimulationSpeed = instance.SimulationSpeed;
    }

    private void OnEnable()
    {
      hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy | HideFlags.HideInInspector;
    }

    public void Save()
    {
      Save(false);
    }
  }
}