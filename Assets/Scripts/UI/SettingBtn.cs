using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingBtn : MonoBehaviour
{
    public GameObject SoundSetting;
    public GameObject GraphicsSetting;
    public TextMeshProUGUI SoundText;
    public TextMeshProUGUI GraphicsText;

    // Start is called before the first frame update
    void Start()
    {
        SoundSetting.SetActive(true);
        GraphicsSetting.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SoundAppear()
    {
        //SoundText.color = new Color(0.608108f, 0, 0);
        //GraphicsText.color = new Color(0.3529412f, 0.5803922f, 0);
        SoundSetting.SetActive(true);
        GraphicsSetting.SetActive(false);
    }
    public void GraphicsAppear()
    {
        //SoundText.color = new Color(0.3529412f, 0.5803922f, 0);
        //GraphicsText.color = new Color(0.608108f, 0, 0);
        SoundSetting.SetActive(false);
        GraphicsSetting.SetActive(true);
    }
}
