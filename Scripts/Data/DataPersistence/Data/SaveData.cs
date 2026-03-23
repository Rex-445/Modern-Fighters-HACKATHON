using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class SaveData
{
    public int slot = 0;
    public int currentLevel = 1;
    public bool showTutorialScreen = true;

    public SaveData()
    {
        slot = 0;
        currentLevel = 1;
        showTutorialScreen = true;
    }
}
