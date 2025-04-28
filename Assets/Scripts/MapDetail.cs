using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDetail : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        transform.position += new Vector3(h, 0, 0) * Time.deltaTime;
    }
}
