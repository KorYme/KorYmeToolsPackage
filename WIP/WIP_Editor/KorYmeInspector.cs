using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using KorYmeLibrary.Attributes;

namespace KorYmeLibrary
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class KorYmeInspector : Editor
    {
        IEnumerable<MethodInfo> _allMethods;

        protected virtual void OnEnable()
        {
            _allMethods = ReflectionUtility.GetAllMethods(target, m => m.GetCustomAttributes(typeof(MethodAttribute), true).Length > 0);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawButtons();
        }

        protected void DrawButtons()
        {
            UnityEngine.Debug.Log("Hey");
            foreach (MethodInfo method in _allMethods)
            {
                KorYmeEditorGUI.Button(serializedObject.targetObject, method);
            }
        }
    }
}
