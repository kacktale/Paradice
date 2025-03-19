using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDetail : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool CanZoom = true;
    private TextMeshProUGUI text;
    private Color mainColor;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CanZoom)
        {
            mainColor = text.color;
            text.DOColor(new Color(0, 0.5058824f, 0.8313726f),1);
            text.gameObject.transform.DOScale(2.5f,1);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(CanZoom)
        {
            text.DOColor(mainColor, 1);
            text.gameObject.transform.DOScale(2.24f, 1);
        }
    }
}
