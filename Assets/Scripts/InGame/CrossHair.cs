using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    public Sprite[] Crosshair;
    private SpriteRenderer SpriteRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 MousePos = Camera.main.ScreenToWorldPoint( Input.mousePosition );
        transform.position = MousePos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SpriteRenderer.sprite = Crosshair[1];
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        SpriteRenderer.sprite = Crosshair[0];
    }
}
