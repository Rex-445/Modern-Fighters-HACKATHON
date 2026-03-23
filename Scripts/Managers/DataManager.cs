using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Playables;
using System;

public class DataManager : MonoBehaviour, IDataPersistence
{
    public string difficulty;
    public int highestScore;
    public int score;


    public int enemies;
    public int barrels;

    public GameObject endingBanner;

    public int keysCollected;

    public static DataManager instance;

    [Header("Player Attributes")]
    public float maxExp;
    public float currentExp;
    public int unitLevel;
    public float unitDamage;

    private FileDataHandler fileData;

    internal DataPersistenceManager dataPersistence;


    [Space(30)]
    [Header("LevelUp Variables")]
    public Animator levelUpAnim;
    public AudioSource levelUpSound;
    bool canLevelUp;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        dataPersistence = GetComponent<DataPersistenceManager>();
    }

    internal void StartGame()
    {
        Invoke("UpdatePlayerAttributes", .01f);
        levelUpSound.ignoreListenerPause = true;/*
        highestScore = dataPersistence.gameData.highScore;*/

        GameObject.FindGameObjectWithTag("UIManager").transform.Find("DisableUI").Find("Top").Find("HighestScore").GetComponent<Text>().text = "HIGHESTSCORE\n" + highestScore;

        if (highestScore == 0)
            GameObject.FindGameObjectWithTag("UIManager").transform.Find("DisableUI").Find("Top").Find("HighestScore").GetComponent<Text>().enabled = false;
    }

    private void Update()
    {
        if (canLevelUp && UnitManager.instance.player.IsGrounded())
        {
            canLevelUp = false;
            LevelUp();
        }
    }

    void UpdatePlayerAttributes()
    {
        Unit player = UnitManager.instance.player;
        unitDamage = player.transform.Find("Container").Find("Sprite").Find("Hitbox").GetComponent<Hitbox>().damage;
        player.unitLevel = dataPersistence.gameData.unitLevel;

        //Reset Player Health
        player.health = player.baseHealth;
        //Add depending on level
        for (int i = 1; i < player.unitLevel; i++)
        {
            player.health += (int)(player.baseHealth * .1f);
        }
    }

public void AddExp(float value)
    {
        currentExp += value;
        if (currentExp >= maxExp)
        {
            canLevelUp = true;
        }
    }

    public void LevelUp()
    {
        Unit player = UnitManager.instance.player;
        //Unit Attributes Change
        unitDamage += (int)(player.baseDamage * .1f);

        //Get the remaining exp
        float remainingExp = currentExp - maxExp;

        //Add the max
        maxExp += (int)(900 * .045f);

        //Reset the current exp
        currentExp = remainingExp;

        unitLevel++;
        player.unitLevel++;

        //VFX and SFX
        levelUpAnim.Play("LevelUp", 0, 0);
        levelUpSound.Play();
        //Hero VFX
        UnitManager.instance.player.transform.Find("Container").Find("Sprite").Find("LevelUp_VFX").gameObject.SetActive(false);
        UnitManager.instance.player.transform.Find("Container").Find("Sprite").Find("LevelUp_VFX").gameObject.SetActive(true);


        //Time
        if (UnitManager.instance.boss == null)
            TimeManager.instance.RealStepPause(.5f);


        //Make a Recursive Call If the current exp is still greater than the maxEXP
        if (currentExp > maxExp)
            LevelUp();
    }

    public void SetDifficulty(string difficultyType)
    {
        dataPersistence.gameData.difficulty = difficultyType;
    }

    public void SetScore()
    {
        if (highestScore < score)
        {
            highestScore = score;
        }
    }

    public void SetEnemy()
    {
        PlayerPrefs.SetInt("EnemiesBeaten", PlayerPrefs.GetInt("EnemiesBeaten", 0) + 1);
    }

    public void SetBarrel()
    {
        PlayerPrefs.SetInt("BarrelsDestroyed", barrels);
    }

    public void Reset()
    {
        dataPersistence.ResetGameSlot();
    }

    public void LoadData(GameData gameData, SaveData saveData)
    {
        unitLevel = gameData.unitLevel;
        maxExp = gameData.unitMaxExp;

        //Set Exp Values
        for (int i = 1; i < unitLevel; i++)
        {
            maxExp += Mathf.Round(maxExp / 2);
        }

        maxExp = gameData.unitMaxExp;

        difficulty = gameData.difficulty;
        highestScore = gameData.highScore;

        UpdateUIData(gameData, saveData);
    }

    private void UpdateUIData(GameData gameData, SaveData saveData)
    {
        if (endingBanner != null)
        {
            endingBanner.transform.Find("EnemiesBeatenText").Find("Text").GetComponent<Text>().text = "" + gameData.enemiesBeaten;
            endingBanner.transform.Find("BarrelsDestroyed").Find("Text").GetComponent<Text>().text = "" + gameData.barrelsDestroyed;
            endingBanner.transform.Find("HIGH SCORE").Find("Value").GetComponent<Text>().text = "" + gameData.highScore;
        }
    }

    public void SaveData(ref GameData gameData, ref SaveData saveData)
    {
        //Set Everything Necessary
        gameData.unitLevel = unitLevel;
        gameData.unitMaxExp = maxExp;
        gameData.playtime += TimeManager.GetPlayTime();

        try
        {

            gameData.lastPlayedLevel = LevelManager.instance.levelID;
        }
        catch { }

        //Set the 
        gameData.highScore = highestScore;
        UpdateUIData(gameData, saveData);
    }

    public void FirstSave(ref GameData data, ref SaveData saveData)
    {
        data.playtime = 0;
    }
}
