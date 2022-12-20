using System.Collections.Generic;
using System;
using ToolLibrary;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Reflection;

public class PrefabsModificationsEditorWindow : EditorWindow
{
    Vector2 _scrollPos;

    public enum PREFABS_FOLDERS
    {
        PREFABS,
        RESOURCES,
        BOTH,
        OTHER,
    }

    public PREFABS_FOLDERS _prefabsFolders;
    public List<GameObject> _prefabs = new();
    public List<Type> _allTypes = new();


    SerializedObject _thisObject;
    SerializedProperty _folderPathProperty;

    [MenuItem("Tools/KorYmeTools/Prefabs Modifications")]
    public static void OpenWindow()
    {
        PrefabsModificationsEditorWindow window = CreateWindow<PrefabsModificationsEditorWindow>("Prefabs Modifications");
    }

    private void OnEnable()
    {
        _thisObject = new(this);
        _folderPathProperty = _thisObject.FindProperty("_prefabsFolders");
        InitTypes();
    }

    private void InitTypes()
    {
        _allTypes.Clear();
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (MemberInfo member in type.GetMembers(UtilitiesClass.FLAGS_FIELDS))
                {
                    if (member.GetCustomAttribute<SceneModificationAttribute>() != null)
                    {
                        if (!_allTypes.Contains(type))
                        {
                            _allTypes.Add(type);
                        }
                    }
                }
            }
        }
    }

    private void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView( _scrollPos );
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Prefabs Modifications", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_folderPathProperty, new GUIContent("Prefabs Folders"));
        if (EditorGUI.EndChangeCheck())
        {
            _thisObject.ApplyModifiedProperties();
            UpdateList();
        }
        EditorGUILayout.Space(5);
        if (GUILayout.Button("Refresh"))
        {
            UpdateList();
        }

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("AllPrefabs", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        EditorGUI.BeginChangeCheck();
        foreach (GameObject gameObject in _prefabs)
        {
            using (new EditorGUI.DisabledScope(true)) EditorGUILayout.ObjectField("GameObject", gameObject, typeof(GameObject), false);
        }
        EditorGUI.EndChangeCheck();
        EditorGUILayout.EndScrollView();
    }

    void UpdateList()
    {
        _prefabs.Clear();
        switch (_prefabsFolders)
        {
            case PREFABS_FOLDERS.PREFABS:
                AddToPrefabsByPath("Prefabs/");
                return;
            case PREFABS_FOLDERS.RESOURCES:
                AddToPrefabsByResources();
                return;
            case PREFABS_FOLDERS.BOTH:
                AddToPrefabsByPath("Prefabs/");
                AddToPrefabsByResources();
                return;
            default:
                return;
        }
    }

    void AddToPrefabsByResources()
    {
        GameObject[] tmp = Resources.LoadAll<GameObject>("");
        for (int i = 0; i < tmp.Length; i++)
        {
            _prefabs.Add(tmp[i]);
        }
    }

    void AddToPrefabsByPath(string path)
    {
        string[] search_results = System.IO.Directory.GetFiles($"Assets/{path}", "*.prefab", System.IO.SearchOption.AllDirectories);
        for (int i = 0; i < search_results.Length; i++)
        {
            _prefabs.Add((GameObject)AssetDatabase.LoadAssetAtPath(search_results[i], typeof(GameObject)));
        }
    }
}
