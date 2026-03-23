using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataPath, string dataName)
    {
        this.dataDirPath = dataPath;
        this.dataFileName = dataName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //deserialize
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }

            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            //create 
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //serialize
            string dataToStore = JsonUtility.ToJson(data, true);

            //write
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }

        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }



    public SaveData LoadSaveFile()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        SaveData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //deserialize
                loadedData = JsonUtility.FromJson<SaveData>(dataToLoad);
            }

            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    public void SaveFile(SaveData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            //create 
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //serialize
            string dataToStore = JsonUtility.ToJson(data, true);

            //write
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }

        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
}
