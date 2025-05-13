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

            Vector2 Direction = Target.transform.position - transform.position;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Direction);
            Debug.DrawRay(transform.position, Direction);

            if (hit.collider != null)
            {
                //Debug.Log(hit.transform.position);
                Indicator.transform.position = hit.point;
            }
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
