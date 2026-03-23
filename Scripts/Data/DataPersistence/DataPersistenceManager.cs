using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DataPersistenceManager : MonoBehaviour
{
    public GameData gameData;
    internal SaveData saveData;

    internal string fileName = "PlayerData_Slot0.json";
    internal string saveFileName = "SaveData.json";
    private FileDataHandler dataHandler;
    private FileDataHandler saveDataHandler;

    public List<IDataPersistence> dataPersistenceObjects;
    public static DataPersistenceManager instance { get; private set; }

    public int SlotID;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            print("Another instance of DataPersistenceManager is running as: " + this.name);
        }

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        saveDataHandler = new FileDataHandler(Application.persistentDataPath, saveFileName);

        dataPersistenceObjects = FindAllDataPersistenceObjects();

        UpdateSlots();
    }

    public void ResetGameSlot()
    {
        fileName = "PlayerData_Slot" + SlotID + ".json";
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        NewGame();
        UpdateSlots();
        print("Game " + "PlayerData_Slot '" + SlotID + "'.json Was Reset");
        //LoadGame();
    }

    public void SetSlot(int ID)
    {
        saveData.slot = ID;
        saveDataHandler.SaveFile(saveData);
        UpdateSlots();
        SlotID = ID;
    }

    public void UpdateSlots()
    {
        //First Slot
        fileName = "PlayerData_Slot0.json";
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        LoadGame();

        //Second Slot
        fileName = "PlayerData_Slot1.json";
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        LoadGame();

        //Third Slot
        fileName = "PlayerData_Slot2.json";
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        LoadGame();

        //Target Slot
        fileName = "PlayerData_Slot" + saveData.slot + ".json";
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        saveDataHandler = new FileDataHandler(Application.persistentDataPath, saveFileName);
        LoadGame();
    }

    public GameData CheckData(int ID)
    {
        //Get The required data and then return it
        fileName = "PlayerData_Slot" + ID + ".json";
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        saveDataHandler = new FileDataHandler(Application.persistentDataPath, saveFileName);
        LoadGame();

        Invoke("UpdateSlots", .1f);
        return gameData;
    }

    private void Start()
    {
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
        NewSaveFiles();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();
        this.saveData = saveDataHandler.LoadSaveFile();

        //Make New
        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initilizing defaults.");
            NewGame();
        }

        //Make New
        if (this.saveData == null)
        {
            this.saveData = new SaveData();
        }

/*
        print(saveData.currentLevel);*/

        //Push to other Scripts
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData, saveData);
        }
    }

    public void SaveGame()
    {
        //Push to other Scripts
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData, ref saveData);
        }

        //Save that data to a file using datamanager
        dataHandler.Save(gameData);
        saveDataHandler.SaveFile(saveData);
    }

    public void NewSaveFiles()
    {
        //Push to other Scripts
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.FirstSave(ref gameData, ref saveData);
        }

        //Save that data to a file using datamanager
        dataHandler.Save(gameData);
        saveDataHandler.SaveFile(saveData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistence = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistence);
    }
}
