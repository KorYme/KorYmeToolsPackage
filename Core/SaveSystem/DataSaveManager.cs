using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using KorYmeLibrary.Attributes;

namespace KorYmeLibrary.SaveSystem
{
    public class DataSaveManager : MonoBehaviour
    {
        public static DataSaveManager Instance { get; private set; }

        private GameDataSample _gameData = null;
        private List<IDataSave> _allSaveData;
        private FileDataHandler _fileDataHandler;

        [Header("File Storage Config")]
        [SerializeField] string _fileName;
        [SerializeField] FileDataHandler.EncryptionType _encryptionType;

        [Header("InGame parameters")]
        [SerializeField] bool _saveOnQuit = false;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There is more than one DataSaveManager in the scene");
                return;
            }
            Instance = this;
            _allSaveData = FindObjectsOfType<MonoBehaviour>().OfType<IDataSave>().ToList();
            _fileDataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _encryptionType);
        }
        private void OnApplicationQuit()
        {
            if (_saveOnQuit)
            {
                SaveGame();
            }
        }

        //Start is used to debug
        private void Start()
        {
            LoadGame();
        }

        public void NewGame()
        {
            _gameData = new GameDataSample();
        }

        public void LoadGame()
        {
            _gameData = _fileDataHandler.Load();
            if (_gameData == null)
            {
                Debug.LogWarning("No data was found. Initializing with defaults data.");
                NewGame();
            }
            _allSaveData.ForEach(x => x.LoadData(_gameData));
        }

        public void SaveGame()
        {
            _allSaveData.ForEach(x => x.SaveData(ref _gameData));
            _fileDataHandler.Save(_gameData);
        }

        [Button(nameof(DestroyOldData))]
        public void DestroyOldData()
        {
            FileDataHandler.DestroyOldData();
        }
    }
}
