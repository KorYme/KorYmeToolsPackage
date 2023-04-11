using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using UnityEngine.UI;
using KorYmeLibrary.Attributes;

namespace KorYmeLibrary.SceneModification
{
    public class SceneModificationsEditorWindow : EditorWindow
    {
        #region PARAMETERS
        bool _isLoaded;
        Vector2 _scrollPos;
        bool _areParametersOpened;
        Color _colorGameObjectLabel;
        Color _colorComponentLabel;
        ObjectLocation _location;
        #endregion

        #region CONTAINERS
        class Node
        {
            public GameObject _go;
            public bool _isOriginal;
            public List<Node> _allChilds;

            public Node(GameObject go, bool isOriginal, List<Node> nodes)
            {
                _go = go;
                _isOriginal = isOriginal;
                _allChilds = nodes;
            }
        }
        List<Type> _allTypes = new();
        Dictionary<GameObject, Dictionary<Component, Tuple<SerializedObject, List<Tuple<FieldInfo, SceneModificationAttribute>>>>> _allComponentsInScene = new();
        Dictionary<GameObject, bool> _foldoutGameObject = new();
        Dictionary<Component, bool> _foldoutComponent = new();
        List<Node> _allNodes = new();
        #endregion

        #region UNITY_METHODS
        [MenuItem("Tools/KorYmeTools/WIP/Scene Modifications")]
        public static void OpenWindow()
        {
            SceneModificationsEditorWindow window = CreateWindow<SceneModificationsEditorWindow>("Scene Modifications");
        }

        public void OnEnable()
        {
            InitAllContainers();
            _location = ObjectLocation.Both;
            if (_colorComponentLabel == Color.clear && _colorGameObjectLabel == Color.clear)
            {
                _colorGameObjectLabel = Color.red;
                _colorComponentLabel = Color.blue;
            }
        }

        private void OnHierarchyChange() => _isLoaded = false;

        private void OnProjectChange() => _isLoaded = false;

        private void OnGUI()
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open All"))
            {
                OpenAllFoldout(true);
            }
            if (GUILayout.Button("Close All"))
            {
                OpenAllFoldout(false);
            }
            EditorGUILayout.EndHorizontal();
            if (_areParametersOpened = EditorGUILayout.Foldout(_areParametersOpened, "[PARAMETERS]", true, UtilitiesClass.ToRichText("foldout")))
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                _location = (ObjectLocation)EditorGUILayout.EnumPopup("Objects locations", _location);
                if (EditorGUI.EndChangeCheck())
                {
                    InitDictionary();
                }
                _colorGameObjectLabel = EditorGUILayout.ColorField(new GUIContent("Parameters color"), _colorGameObjectLabel);
                _colorComponentLabel = EditorGUILayout.ColorField(new GUIContent("Parameters color"), _colorComponentLabel);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField($"[ALLFIELDS({_location})]", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            //if (!_isHierarchised)
            //{
            //    foreach (GameObject gameObject in _allComponentsInScene.Keys.ToArray().OrderBy(x => x.transform.GetSiblingIndex()))
            //    {
            //        EditorGUILayout.Space(10);
            //        if (_foldoutGameObject[gameObject] = EditorGUILayout.Foldout(_foldoutGameObject[gameObject],
            //            UtilitiesClass.DisplayTextWithColours("[GAMEOBJECT] ", _colorGameObjectLabel) + gameObject.name, true, UtilitiesClass.ToRichText("foldout")))
            //        {
            //            EditorGUI.indentLevel++;
            //            EditorGUILayout.Space(10);
            //            using (new EditorGUI.DisabledScope(true)) EditorGUILayout.ObjectField("GameObject",
            //                gameObject, typeof(GameObject), false);
            //            foreach (Component component in _allComponentsInScene[gameObject].Keys.ToArray().OrderBy(x => x.name))
            //            {
            //                EditorGUILayout.Space(2);
            //                if (_foldoutComponent[component] = EditorGUILayout.Foldout(_foldoutComponent[component],
            //                    UtilitiesClass.DisplayTextWithColours("[COMPONENT] ", _colorComponentLabel) + component.GetType().ToString(),
            //                    true, UtilitiesClass.ToRichText("foldout")))
            //                {
            //                    EditorGUI.indentLevel++;
            //                    foreach (Tuple<FieldInfo, SceneModificationAttribute> tuple in _allComponentsInScene[gameObject][component].Item2)
            //                    {
            //                        if ((tuple.Item2._objectLocation == ObjectLocation.Project && gameObject.scene.name != null) || 
            //                            (tuple.Item2._objectLocation == ObjectLocation.Scene && gameObject.scene.name == null)) continue;
            //                        SerializedProperty serializedProperty = _allComponentsInScene[gameObject][component].Item1.FindProperty(tuple.Item1.Name);
            //                        EditorGUI.BeginChangeCheck();
            //                        EditorGUILayout.PropertyField(serializedProperty, new GUIContent(UtilitiesClass.FieldName(tuple.Item1.Name)));
            //                        if (EditorGUI.EndChangeCheck())
            //                        {
            //                            _allComponentsInScene[gameObject][component].Item1.ApplyModifiedProperties();
            //                        }
            //                    }
            //                    EditorGUI.indentLevel--;
            //                }
            //            }
            //            EditorGUI.indentLevel--;
            //        }
            //    }
            //}
            if (_isLoaded)
            {
                DisplayFirstObject();
            }
            else
            {
                if (GUILayout.Button("Reload"))
                {
                    CheckAllContainers();
                    _isLoaded = true;
                }
            }
            EditorGUILayout.Space(30);
            EditorGUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
        #endregion

        #region INITIALIZATIONS

        private void InitAllContainers()
        {
            InitTypes();
            InitDictionary();
            InitHierarchyTree();
            _scrollPos = Vector2.zero;
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
    
        private void InitDictionary()
        {
            _allComponentsInScene.Clear();
            _foldoutComponent.Clear();
            _foldoutGameObject.Clear();
            List<Component> allComponents = new();
            foreach (Type type in _allTypes)
            {
                foreach (Component component in FindObjectsOfType(type))
                {
                    allComponents.Add(component);
                }
                string[] search_results = System.IO.Directory.GetFiles("Assets/", "*.prefab", System.IO.SearchOption.AllDirectories);
                for (int i = 0; i < search_results.Length; i++)
                {
                    foreach (Component component in ((GameObject)AssetDatabase.LoadAssetAtPath(search_results[i], typeof(GameObject))).GetComponents(type))
                    {
                        allComponents.Add(component);
                    }
                }
                foreach (Component component in allComponents)
                {
                    List<Tuple<FieldInfo, SceneModificationAttribute>> allFieldsInScene = new();
                    foreach (MemberInfo member in component.GetType().GetMembers(UtilitiesClass.FLAGS_FIELDS))
                    {
                        if (member.CustomAttributes.ToArray().Length > 0)
                        {
                            SceneModificationAttribute attribute = member.GetCustomAttribute<SceneModificationAttribute>();
                            if (attribute != null)
                            {
                                if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
                                {
                                    allFieldsInScene.Add(Tuple.Create((FieldInfo)member, attribute));
                                }
                            }
                        }
                    }
                    if (allFieldsInScene.Count > 0)
                    {
                        if (!_allComponentsInScene.ContainsKey(component.gameObject))
                        {
                            _allComponentsInScene.Add(component.gameObject, new());
                            _foldoutGameObject.Add(component.gameObject, false);
                        }
                        _allComponentsInScene[component.gameObject].Add(component, Tuple.Create(new SerializedObject(component), allFieldsInScene));
                        _foldoutComponent.Add(component, false);
                    }
                }
            }
        }

        private void InitHierarchyTree()
        {
            _allNodes.Clear();
            foreach (GameObject go in _allComponentsInScene.Keys.ToArray())
            {
                _allNodes.Add(new Node(go, true, new()));
            }
            for (int i = 0; i < _allNodes.Count; i++)
            {
                Transform tempParent = _allNodes[i]._go.transform.parent;
                while (tempParent != null && _allNodes[i]._isOriginal)
                {
                    if (_allComponentsInScene.Keys.ToList().Contains(tempParent.gameObject))
                    {
                        _allNodes[i]._isOriginal = false;
                        _allNodes.Find(x=>x._go == tempParent.gameObject)._allChilds.Add(_allNodes[i]);
                    }
                    tempParent= tempParent.parent;
                }
            }
        }
        #endregion

        #region CHECKS
        private void CheckAllContainers()
        {
            CheckDictionnary();
            InitHierarchyTree();
            _scrollPos = Vector2.zero;

        }

        private void CheckDictionnary()
        {
            RemoveNullKeysDictionary();
            AddNewKeysDictionary();
        }

        private void RemoveNullKeysDictionary()
        {
            foreach (GameObject gameObject in _allComponentsInScene.Keys.ToArray())
            {
                if (gameObject == null)
                {
                    _allComponentsInScene.Remove(gameObject);
                    _foldoutGameObject.Remove(gameObject);
                }
                foreach (Component component in _allComponentsInScene[gameObject].Keys.ToArray())
                {
                    if (component == null)
                    {
                        _allComponentsInScene[gameObject].Remove(component);
                        _foldoutComponent.Remove(component);
                    }
                }
            }
        }

        private void AddNewKeysDictionary()
        {
            foreach (Type type in _allTypes)
            {
                foreach (Component component in FindObjectsOfType(type))
                {
                    if ((_allComponentsInScene.ContainsKey(component.gameObject) ? !_allComponentsInScene[component.gameObject].ContainsKey(component) : false) ||
                        !_allComponentsInScene.ContainsKey(component.gameObject))
                    {
                        List<Tuple<FieldInfo,SceneModificationAttribute>> allFields = new();
                        foreach (MemberInfo member in component.GetType().GetMembers(UtilitiesClass.FLAGS_FIELDS))
                        {
                            if (member.CustomAttributes.ToArray().Length > 0)
                            {
                                SceneModificationAttribute attribute = member.GetCustomAttribute<SceneModificationAttribute>();
                                if (attribute != null)
                                {
                                    if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
                                    {
                                        allFields.Add(Tuple.Create((FieldInfo)member, attribute));
                                    }
                                }
                            }
                        }
                        if (!_allComponentsInScene.ContainsKey(component.gameObject))
                        {
                            _allComponentsInScene.Add(component.gameObject, new());
                            _foldoutGameObject.Add(component.gameObject, false);
                        }
                        _allComponentsInScene[component.gameObject].Add(component, Tuple.Create(new SerializedObject(component), allFields));
                        _foldoutComponent.Add(component, false);
                    }
                }
            }
        }
        #endregion

        #region GUI_METHODS
        private void OpenAllFoldout(bool opened)
        {
            foreach (Component item in _foldoutComponent.Keys.ToArray())
            {
                _foldoutComponent[item] = opened;
            }
            foreach (GameObject item in _foldoutGameObject.Keys.ToArray())
            {
                _foldoutGameObject[item] = opened;
            }
        }

        private void DisplayFirstObject()
        {
            foreach (Node node in _allNodes.Where(x => x._isOriginal).OrderBy(x => x._go.transform.GetSiblingIndex()))
            {
                EditorGUILayout.Space(10);
                if (_foldoutGameObject[node._go] = EditorGUILayout.Foldout(_foldoutGameObject[node._go],
                    UtilitiesClass.DisplayTextWithColours("[GAMEOBJECT] ", _colorGameObjectLabel) + node._go.name, true, UtilitiesClass.ToRichText("foldout")))
                {
                    EditorGUI.indentLevel++;
                    using (new EditorGUI.DisabledScope(true)) EditorGUILayout.ObjectField("GameObject", node._go, typeof(GameObject), false);
                    foreach (Component component in _allComponentsInScene[node._go].Keys.ToArray().OrderBy(x => x.name))
                    {
                        EditorGUILayout.Space(2);
                        if (_foldoutComponent[component] = EditorGUILayout.Foldout(_foldoutComponent[component],
                            UtilitiesClass.DisplayTextWithColours("[COMPONENT] ", _colorComponentLabel) + component.GetType().ToString(),
                            true, UtilitiesClass.ToRichText("foldout")))
                        {
                            EditorGUI.indentLevel++;
                            foreach (Tuple<FieldInfo, SceneModificationAttribute> tuple in _allComponentsInScene[node._go][component].Item2)
                            {
                                if ((tuple.Item2._objectLocation == ObjectLocation.Project && node._go.scene.name != null) ||
                                    (tuple.Item2._objectLocation == ObjectLocation.Scene && node._go.scene.name == null)) continue;
                                SerializedProperty serializedProperty = _allComponentsInScene[node._go][component].Item1.FindProperty(tuple.Item1.Name);
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(UtilitiesClass.FieldName(tuple.Item1.Name)));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    _allComponentsInScene[node._go][component].Item1.ApplyModifiedProperties();
                                }
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                    DisplayChildGameObjects(node._allChilds);
                    EditorGUI.indentLevel--;
                }
            }
        }

        private void DisplayChildGameObjects(List<Node> allChilds)
        {
            foreach (Node node in allChilds.OrderBy(x => x._go.transform.GetSiblingIndex()))
            {
                EditorGUILayout.Space(10);
                if (_foldoutGameObject[node._go] = EditorGUILayout.Foldout(_foldoutGameObject[node._go],
                    UtilitiesClass.DisplayTextWithColours("[GAMEOBJECT] ", _colorGameObjectLabel) + node._go.name, true, UtilitiesClass.ToRichText("foldout")))
                {
                    EditorGUI.indentLevel++;
                    using (new EditorGUI.DisabledScope(true)) EditorGUILayout.ObjectField("GameObject", node._go, typeof(GameObject), false);
                    foreach (Component component in _allComponentsInScene[node._go].Keys.ToArray().OrderBy(x => x.name))
                    {
                        EditorGUILayout.Space(2);
                        if (_foldoutComponent[component] = EditorGUILayout.Foldout(_foldoutComponent[component], UtilitiesClass.DisplayTextWithColours("[COMPONENT] ",
                            _colorComponentLabel) + component.GetType().ToString(), false, UtilitiesClass.ToRichText("foldout")))
                        {
                            EditorGUI.indentLevel++;
                            foreach (Tuple<FieldInfo,SceneModificationAttribute> tuple in _allComponentsInScene[node._go][component].Item2)
                            {
                                SerializedProperty serializedProperty = _allComponentsInScene[node._go][component].Item1.FindProperty(tuple.Item1.Name);
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(UtilitiesClass.FieldName(tuple.Item1.Name)));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    _allComponentsInScene[node._go][component].Item1.ApplyModifiedProperties();
                                }
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                    DisplayChildGameObjects(node._allChilds);
                    EditorGUI.indentLevel--;
                }
            }
        }
        #endregion
    }
}