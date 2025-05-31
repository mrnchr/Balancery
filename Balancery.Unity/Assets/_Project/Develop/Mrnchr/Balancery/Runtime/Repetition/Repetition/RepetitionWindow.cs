using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Mrnchr.Balancery.Editor
{
  public class RepetitionWindow : EditorWindow
  {
    private UnityEditor.Editor _editor;

    protected virtual RepetitionPreferences Preferences => RepetitionPreferences.instance;

    [MenuItem("Window/Balancery/Repetition Window")]
    public static void GetOrCreateWindow()
    {
      var window = GetWindow<RepetitionWindow>();
      window.SetName();
      window.Show();
    }

    protected virtual void SetName()
    {
      titleContent = new GUIContent(ObjectNames.NicifyVariableName(GetType().Name.Replace("Window", "")));
    }

    private void OnEnable()
    {
      Subscribe();
      RecreateGUI();
    }

    private void Subscribe()
    {
      SceneManager.activeSceneChanged += OnActiveSceneChanged;
      EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
      RecreateGUI();
    }

    private void Unsubscribe()
    {
      SceneManager.activeSceneChanged -= OnActiveSceneChanged;
      EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChanged;
    }

    private void RecreateGUI()
    {
      if (_editor)
        DestroyImmediate(_editor);

      _editor = UnityEditor.Editor.CreateEditor(Preferences);
      CreateGUI();
    }

    private void CreateGUI()
    {
      rootVisualElement.Clear();
      rootVisualElement.hierarchy.Add(new ScrollView(ScrollViewMode.Vertical));
      var scrollView = rootVisualElement.Q<ScrollView>();
      scrollView.Add(new IMGUIContainer(() => _editor.DrawHeader()));
      scrollView.Add(new InspectorElement(_editor));
    }

    private void Update()
    {
      if (EditorUtility.IsDirty(Preferences))
      {
        Preferences.Save();
      }
    }

    private void DisposeInternalData()
    {
      if (_editor)
        DestroyImmediate(_editor);
    }

    private void OnDisable()
    {
      DisposeInternalData();
      Unsubscribe();
    }
  }
}