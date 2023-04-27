using KorYmeLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CodeGenerator : MonoBehaviour
{
    [SerializeField] List<SaveVariable> _variableName;
    [SerializeField] Quaternion _testValue;
    [SerializeField] string _className = "MyGameData";
    [SerializeField] string _folderName = "SaveSystemClasses";
    [SerializeField, TextArea(15, 10)] string _classCode;

    [Button]
    public void GenerateClass()
    {
        //string classCode =
        //    "namespace KorYmeLibrary.SaveSystem " + "\n" +
        //    "{" + "\n" +
        //    "   [System.Serializable]" + "\n" +
        //    "   public class " + _className + "\n" +
        //    "   {" + "\n" +
        //    "   }" + "\n" +
        //    "}";


        Debug.Log("lance");
        if (_variableName.TrueForAll(x => _variableName.FindAll(y => y.Name == x.Name).Count == 1))
        {
            Debug.Log("There can't be two variables named the same way.");
            return;
        }
        Debug.Log("reussi");
        string folderPath = Application.dataPath + "/" + _folderName;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        // Écriture du code généré dans un fichier
        string path = folderPath + "/" + _className + ".cs";
        File.WriteAllText(path, _classCode);
        AssetDatabase.Refresh();
    }

    enum AllSerializedTypes
    {
        Boolean,
        Int,
        Float,
        Double,
        String,
        Vector2,
        Vector3,
        Quaternion,
        Color,
        //ScriptableObject à faire si possible
    }

    [System.Serializable]
    struct SaveVariable
    {
        public AllSerializedTypes Type;
        public string Name;
    }
}