using KorYmeLibrary.Attributes;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace KorYmeLibrary.SaveSystem
{
    public class DataSaveSystemGenerator : MonoBehaviour
    {
        [SerializeField] string _dataClassName = "MyGameData";
        string _folderName = "SaveSystemClasses";

        [Button]
        public void GenerateGameDataClass()
        {
            string folderPath = Application.dataPath + "/" + _folderName;
            string systemClassName = "DSM_" + _dataClassName;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }

            if (File.Exists(folderPath + "/" + systemClassName + ".cs"))
            {
                Debug.LogWarning("There is already one class named this way in " + _folderName);
                return;
            }

            // �criture du code g�n�r� dans un fichier
            string path = folderPath + "/" + systemClassName + ".cs";
            string classCode =
                "namespace KorYmeLibrary.SaveSystem " + "\n" +
                "{" + "\n" +
                "   public class " + systemClassName + " : DataSaveManager<" + _dataClassName + ">" + "\n" +
                "   {" + "\n" +
                "   }" + "\n" +
                "\n" +
                "   [System.Serializable]" + "\n" +
                "   public class " + _dataClassName + " : " + "GameDataTemplate" + "\n" +
                "   {" + "\n" +
                "       // Create the values you want to save here" + "\n" +
                "   }" + "\n" +
                "}";

            File.WriteAllText(path, classCode);
            AssetDatabase.Refresh();
        }

        [Button]
        void AttachDataSaveManager()
        {
            string folderPath = Application.dataPath + "/" + _folderName;
            string systemClassName = "DSM_" + _dataClassName;
            if (!Directory.Exists(folderPath))
            {
                Debug.LogWarning("No folder SaveSystemClasses has been found");
                return;
            }
            if (!File.Exists(folderPath + "/" + systemClassName + ".cs"))
            {
                Debug.LogWarning("No game data class has been found in the folder : " + folderPath);
                return;
            }
            Type type = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(a => a.GetTypes())
                                .FirstOrDefault(t => t.Name == systemClassName);
            if (type == null)
            {
                Debug.LogWarning("No type has been found");
                return;
            }
            if (GetComponent(type) != null)
            {
                Debug.LogWarning("A component already exists on this gameObject");
                return;
            }
            gameObject.AddComponent(type);
        }
    }
}