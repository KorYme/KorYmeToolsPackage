using KorYmeLibrary.Attributes;
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CodeGenerator : MonoBehaviour
{
    [SerializeField] string _className = "MyGameData";
    [SerializeField] string _folderName = "SaveSystemClasses";
    [SerializeField, TextArea(15, 10)] string _classCode;

    [Button(nameof(GenerateClass))]
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

    //private void Reset()
    //{
    //    GenerateClass();
    //}
}