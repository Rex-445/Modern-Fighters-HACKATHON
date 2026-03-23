using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPointManager : MonoBehaviour, IDataPersistence
{
    public List<CheckPoint> checkpoints;


    public UnityEvent OnBeginWithoutCheckPoints;

    /// <summary>
    /// Making an instance of CheckPointManager
    /// </summary>
    public static CheckPointManager instance;

    public int id;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            checkpoints.Add(transform.GetChild(i).gameObject.GetComponent<CheckPoint>());
            checkpoints[i].ID = checkpoints.Count;
        }




        //Initiate Checkpoint
        if (id > 0)
        {
            Invoke("ActivateCheckPoint", .01f);
        }

        else
        {
            Begin();
        }
    }

    private void Begin()
    {
        OnBeginWithoutCheckPoints.Invoke();
    }

    public void ActivateCheckPoint()
    {
        if (id > checkpoints.Count)
        {
            //Reset the checkpoint system then
            id = 0;
            Begin();
            return;
        }

        checkpoints[id - 1].ActivateCheckPoint();
        Debug.Log("Activating Checkpoint " + id + " already Saved");
    }

    public void LoadData(GameData gData, SaveData sData)
    {
        //Before doing this be sure that this is the same level that the player previously played
        //This is so they can easily return to it and avoid level jumping

        //If the "Last Level Played" based on the Slot Selected is equal to the current level then continue as normal
        if (gData.lastPlayedLevel == sData.currentLevel)
        {
            id = gData.checkPointLevel;
        }

        //If the "Last Level Played" based on the Slot Selected is not equal to the current level then reset the checkpoint
        else
        {
            //After that reset the checkpoint level based on the slot selected
            gData.checkPointLevel = 0;
            gData.lastPlayedLevel = sData.currentLevel;

            //Start a new Game With No Checkpoint
            id = 0;
        }
    }

    public void SaveData(ref GameData data, ref SaveData saveData)
    {
        data.checkPointLevel = id;
    }

    public void FirstSave(ref GameData data, ref SaveData saveData)
    {
        return;
    }
}
