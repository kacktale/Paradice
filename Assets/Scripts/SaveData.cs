using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Data
{
    public int RoomNum = 0;
}

public class SaveData : MonoBehaviour
{
    public Data Jsondata = new Data();
    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILE_NAME = "/SaveFile.txt";

    // Start is called before the first frame update
    void Awake()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/";
        CheckDataFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SaveJsonData()
    {
        string json = JsonUtility.ToJson(Jsondata);
        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILE_NAME, json);
    }

    public void LoadData()
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILE_NAME))
        {
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILE_NAME);
            Jsondata = JsonUtility.FromJson<Data>(loadJson);

            Debug.Log(loadJson);
        }
        else
        {
            
        }
    }

    public void CheckDataFile()
    {
        if (!Directory.Exists(SAVE_DATA_DIRECTORY))
        {
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
            SaveJsonData();
        }
    }
    public void ResetData()
    {
        Jsondata = new Data();
    }
}
