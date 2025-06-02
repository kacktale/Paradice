using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public SaveData SaveData;
    public RectTransform[] transitions;
    public RectTransform Icon;
    private bool IconRotate;
    // Start is called before the first frame update
    void Awake()
    {
        SaveData = GetComponent<SaveData>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IconRotate) Icon.rotation *= Quaternion.Euler(0,0,80 * Time.deltaTime);
    }
    public void NewGame()
    {
        Transition();
        Invoke("ResetData", 4);
    }
    void ResetData()
    {
        SaveData.ResetData();
        SaveData.Jsondata.RoomNum++;
        SceneManager.LoadScene(SaveData.Jsondata.RoomNum);
        SaveData.SaveJsonData();
    }
    public void LoadGame()
    {
        Transition();
        Invoke("LoadData", 3);
    }
    void LoadData()
    {
        SaveData.LoadData();
        SceneManager.LoadScene(SaveData.Jsondata.RoomNum);
    }

    void Transition()
    {
        transitions[0].DOAnchorPos3DX(-480,0.3f);
        transitions[1].DOAnchorPos3DX(480, 0.3f);
        IconRotate = true;
    }
}
