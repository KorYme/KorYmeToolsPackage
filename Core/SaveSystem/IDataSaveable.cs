using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KorYmeLibrary.SaveSystem
{
    public interface IDataSaveable<T>
    {
        void LoadData(T gameData);

        void SaveData(ref T gameData);

        void InitializeData();
    }
}
