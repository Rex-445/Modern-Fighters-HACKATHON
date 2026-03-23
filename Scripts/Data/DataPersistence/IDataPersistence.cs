using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    void LoadData(GameData gameData, SaveData saveData);

    void SaveData(ref GameData data, ref SaveData saveData);

    void FirstSave(ref GameData data, ref SaveData saveData);
}
