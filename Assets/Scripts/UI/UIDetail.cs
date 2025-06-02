using DG.Tweening;
using NovaSamples.Effects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDetail : MonoBehaviour
{
    public GameObject SettingObject;
    public GameObject Outline;
    public BlurEffect BlurEffect;
    public RectTransform Text;
    public Transform Circle;
    public GameObject[] SettingPanels;

    private bool BlurOn = false;
    // Start is called before the first frame update
    void Start()
    {
        Text.DOAnchorPos(Vector2.zero,1).SetEase(Ease.OutQuint);
        Circle.DOMoveX(3.52f,1).SetEase(Ease.OutQuint);
    }

    // Update is called once per frame
    void Update()
    {
        Outline.transform.rotation *= Quaternion.Euler(0, 0, 10 * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SettingPanel();
        }
        if (BlurOn)
        {
            if (BlurEffect.BlurRadius < 90.5f)
                BlurEffect.BlurRadius += 5.4f;
            else
            {
                BlurEffect.BlurRadius = 90.5f;
            }
        }
        else
        {
            if (BlurEffect.BlurRadius > 0)
                BlurEffect.BlurRadius -= 5.4f;
            else
            {
                BlurEffect.BlurRadius = 0f;
            }
        }
    }

    public void SettingPanel()
    {
        if (!BlurOn)
        {
            BlurOn = true;
            SettingPanels[0].SetActive(true);
            Text.gameObject.SetActive(false);
        }
        else
        {
            BlurOn = false;
            SettingPanels[0].SetActive(false);
            Text.gameObject.SetActive(true);
        }
    }
}
