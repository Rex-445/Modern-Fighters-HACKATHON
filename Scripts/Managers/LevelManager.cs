using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int levelID;
    public string levelTitle;

    public static LevelManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        Invoke("InitLevel", 1.5f);
    }

    public void InitLevel()
    {
        StartCoroutine(UIManager.instance.DebugGameTitle(levelTitle));
        DataManager.instance.dataPersistence.saveData.currentLevel = levelID;

    }
}
