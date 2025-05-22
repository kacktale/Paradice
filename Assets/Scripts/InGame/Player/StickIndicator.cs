using UnityEngine;

public class StickIndicator : MonoBehaviour
{
    public GameObject Indicator;
    public GameObject Target;

    Renderer rd;

    private void Start()
    {
        rd = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (rd.isVisible == false)
        {
            if (Indicator.activeSelf == false)
            {
                Indicator.SetActive(true);
            }

            //if(transform.localPosition.x > Indicator.transform.localPosition.x)
            //{
            //    Indicator.transform.position = new Vector3(8.27f, 0, 10);
            //}
            //else
            //{
            //    Indicator.transform.position = new Vector3(-8.27f, 0, 10);
            //}
            float lookdir = Mathf.Atan2(Indicator.transform.position.y - transform.position.y,Indicator.transform.position.x - transform.position.x) * Mathf.Rad2Deg;

            Indicator.transform.rotation = Quaternion.Euler(0,0,lookdir-30);
        }
        else
        {
            if (Indicator.activeSelf == true)
            {
                Indicator.SetActive(false);
            }
        }
    }
}
