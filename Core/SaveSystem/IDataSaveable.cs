using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KorYmeLibrary.SaveSystem
{
    public interface IDataSave
    {
        void LoadData(GameDataSample gameData);

        void SaveData(ref GameDataSample gameData);
    }
}
