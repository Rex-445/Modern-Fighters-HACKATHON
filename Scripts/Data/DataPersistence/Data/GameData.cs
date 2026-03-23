using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class GameData
{
    //Hero Variables
    public int unitLevel;
    public float unitMaxExp;

    //InGame Variables
    public int highScore;
    public string difficulty;
    public int enemiesBeaten;
    public int levelProgress;


    //Player Data Variables
    public bool usedSlot;
    public float playtime;
    public string userName;
    public string userAge;
    public bool isJoystick;
    public int barrelsDestroyed;

    //CheckPoints
    [Space(50)]
    public int checkPointLevel;
    public int lastPlayedLevel;


    public GameData()
    {
        unitLevel = 1;
        unitMaxExp = 900;

        levelProgress = 1;
        lastPlayedLevel = 1;


        //User Values
        userName = "Create New";
        userAge = "-";
        playtime = 0;
        difficulty = "Normal";

        highScore = 0;
        enemiesBeaten = 0;
        barrelsDestroyed = 0;
        usedSlot = false;

        checkPointLevel = 0;
    }
}
