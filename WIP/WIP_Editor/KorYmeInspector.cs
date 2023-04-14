using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KorYmeLibrary
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class KorYmeInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawButtons();
        }
        private void DrawButtons()
        {

        }
    }
}
