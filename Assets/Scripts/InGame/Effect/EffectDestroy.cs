using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroy : MonoBehaviour
{
    public float DeleteTime;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Delete", DeleteTime);
    }
    void Delete()
    {
        Destroy(gameObject);
    }
}
