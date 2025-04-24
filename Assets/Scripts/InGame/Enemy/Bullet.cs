using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bullet : MonoBehaviour
{
    public float Speed;
    public GameObject player;
    private Vector3 targetPosition;
    public bool Recoil = true;


    void Start()
    {
        if (Recoil)
        {
            float RecoilX = Random.Range(-2f, 2f);
            float RecoilY = Random.Range(-2f, 2f);
            Vector3 movePosition = player.transform.position + new Vector3(RecoilX, RecoilY, 0);
            targetPosition = (movePosition - transform.position).normalized;
        }
        else
        {
            Vector3 movePosition = player.transform.position/* + new Vector3 (0,-1,0)*/;
            targetPosition = (movePosition - transform.position).normalized;
        }
        //transform.position += Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += targetPosition * Speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Slow"))
        {
            Speed /= 2;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Slow"))
        {
            Speed *= 2;
        }
    }
}
