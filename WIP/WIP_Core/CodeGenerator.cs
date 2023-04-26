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

public class MenuManager : MonoBehaviour
{
    public GameObject menu; // Ton gameObject de menu

    public void DisplayMenu() 
    {
        menu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RemoveMenu() // Il faudra que tu glisses ton object avec ce script dans l'event de ton bouton "fermer le menu" et que tu appelles cette fonction dedans
        // si tu sais pas comment faire je t'invite à regarder un tuto
    {
        menu.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1f)
            {
                DisplayMenu();
            }
            else
            {
                RemoveMenu();
            }
        }
    }
}