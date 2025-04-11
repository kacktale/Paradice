using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public SaveData SaveData;
    // Start is called before the first frame update
    void Awake()
    {
        SaveData = GetComponent<SaveData>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void NewGame()
    {
        SaveData.ResetData();
        SaveData.Jsondata.RoomNum++;
        SceneManager.LoadScene(SaveData.Jsondata.RoomNum);
        SaveData.SaveJsonData();
    }
    public void LoadGame()
    {
        SaveData.LoadData();
        SceneManager.LoadScene(SaveData.Jsondata.RoomNum);
    }
}
