using System;
using System.IO;
using UnityEngine;

namespace KorYmeLibrary.SaveSystem
{
    public class FileDataHandler<T>
    {
        private string _dataDirPath = "";
        private string _dataFileName = "";
        private EncryptionUtilities.EncryptionType _encryptionType = EncryptionUtilities.EncryptionType.None;

        public FileDataHandler(string dataDirPath, string dataFileName, EncryptionUtilities.EncryptionType encryptionType)
        {
            this._dataDirPath = dataDirPath;
            this._dataFileName = dataFileName;
            this._encryptionType = encryptionType;
        }

        public T Load()
        {
            string fullPath = Path.Combine(_dataDirPath, _dataFileName);
            if (!File.Exists(fullPath)) return default(T);
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = EncryptionUtilities.Encrypt(reader.ReadToEnd(), _encryptionType, false);
                    }
                }
                return JsonUtility.FromJson<T>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error occured when trying to save data to file: " + fullPath + "\n" + e);
                return default(T);
            }
        }

        public void Save(T data)
        {
            string fullPath = Path.Combine(_dataDirPath, _dataFileName);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                string dataToStore = EncryptionUtilities.Encrypt(JsonUtility.ToJson(data, true), _encryptionType, true);
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToStore);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
            }
        }

        public static void DestroyOldData()
        {
            DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
            int fileDestroyed = 0;
            int directoryDestroyed = 0;
            foreach (FileInfo file in di.GetFiles())
            {
                Debug.Log("This file has been deleted  : \n" + file.Name);
                file.Delete();
                fileDestroyed++;
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                Debug.Log("This directory has been deleted  : \n" + dir.Name);
                dir.Delete(true);
                directoryDestroyed++;
            }
            Debug.Log("You have destroyed " + fileDestroyed.ToString() + " files and " + directoryDestroyed.ToString() + " directories.");
        }
    }
}
