using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float MoveSpeed;
    public float JumpForce;
    public GameObject Effect;
    private Rigidbody2D rb;
    public bool IsJump = false;
    private bool CanJump = false;

    private Vector2 PlayerMaxYPos;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsJump)
        {
            if (this.gameObject.transform.position.y > PlayerMaxYPos.y)
            {
                rb.gravityScale = 1f;
                PlayerMaxYPos = this.gameObject.transform.position;
            }
            else
            {
                rb.gravityScale = 0.09f;
            }
        }
        float h = Input.GetAxisRaw("Horizontal");
        transform.position += new Vector3(h, 0) * Time.deltaTime * MoveSpeed;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(CanJump)
            {
                Effect.transform.DOScale(50, 0.1f);
                rb.velocity = new Vector2(rb.velocity.x, JumpForce);
                IsJump = true;
                CanJump = false;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb.gravityScale = 1f;
        PlayerMaxYPos = new Vector2();
        Effect.transform.DOScale(0, 0.1f);
        IsJump = false;
        CanJump = true;
    }
}
