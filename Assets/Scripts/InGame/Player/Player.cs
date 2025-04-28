using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Globalization;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("플레이어 스탯")]
    public float MoveSpeed;
    public float JumpForce;
    public bool IsJump = false;
    public float MaxHealth;
    private float CurHealth;
    public bool Mujuck = false;

    [Header("무기")]
    public GameObject Effect;
    public GameObject Stick;
    public CrossHair CrossHair;
    public int RemainStick;

    [Header("점프 관련")]
    public float jumpBufferTime = 0.1f;
    private bool canCheckGround = true;

    public bool CanDash = true;
    private bool IsDashing;
    public float DashSpeed = 20f;
    public float DashTime = 0.2f;
    public LayerMask WallLayer;

    [Header("이펙트")]
    public CinemachineVirtualCamera VirtualCamera;
    private bool ZoomIn;

    private Rigidbody2D rb;
    private bool CanJump = false;

    private Vector2 PlayerMaxYPos;
    private int defaultLayer;
    public string dashLayerName = "DashOnly";

    void Start()
    {
        CurHealth = MaxHealth;
        rb = GetComponent<Rigidbody2D>();
        RemainStick = 2;
        defaultLayer = gameObject.layer;
    }

    void FixedUpdate()
    {
        if (IsJump)
        {
            if (CanDash)
            {
                if (this.gameObject.transform.position.y > PlayerMaxYPos.y)
                {
                    PlayerMaxYPos = this.gameObject.transform.position;
                }
                else if (this.gameObject.transform.position.y < PlayerMaxYPos.y)
                {
                    //rb.gravityScale = 0.09f;
                }
            }
            else
            {
                rb.gravityScale = 1f;
            }
        }
        if (!IsDashing)
        {
            float h = Input.GetAxisRaw("Horizontal");
            if(h < 0)
            {
                transform.rotation = Quaternion.Euler(0, -180, 0);
            }
            else if(h > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            rb.velocity = new Vector2(h * MoveSpeed, rb.velocity.y);
        }
    }

    private void Update()
    {
        JumpCheck();
        //점프
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //점프가 가능한가
            if (CanJump)
            {
                Jump();
                if (CanDash)
                {
                    Effect.transform.DOScale(500, 0.1f);
                    MoveSpeed = 2.5f;
                    rb.gravityScale = 0.09f;
                }
                PlayerMaxYPos = new Vector2();
            }
        }
        //젓가락 날리기
        if (Input.GetMouseButtonDown(0))
        {
            if (RemainStick > 0)
            {
                GameObject stick;
                StickMove StickCs;
                if (!CanJump)
                {
                    if (transform.rotation == Quaternion.Euler(0, 0, 0))
                    {
                        stick = Instantiate(Stick, transform.position + Vector3.left, Quaternion.identity);
                        StickCs = stick.GetComponent<StickMove>();
                        StickCs.TargetPos = CrossHair.gameObject.transform;
                        StickCs.player = gameObject.transform;
                    }
                    else
                    {
                        stick = Instantiate(Stick, transform.position - Vector3.left, Quaternion.identity);
                        StickCs = stick.GetComponent<StickMove>();
                        StickCs.TargetPos = CrossHair.gameObject.transform;
                        StickCs.player = gameObject.transform;
                    }
                }
                else
                {
                    stick = Instantiate(Stick, transform.position, Quaternion.identity);
                    StickCs = stick.GetComponent<StickMove>();
                    StickCs.TargetPos = CrossHair.gameObject.transform;
                    StickCs.player = gameObject.transform;
                }
                StartCoroutine(StickEffect());
                RemainStick--;
            }
        }
        //대쉬
        if (Input.GetMouseButtonDown(1) && CanDash)
        {
            CanDash = false;
            IsDashing = true;
            StartCoroutine(Dash());
        }
        //카메라 줌(이펙트)
        if (ZoomIn)
        {
            if (VirtualCamera.m_Lens.OrthographicSize > 3)
            {
                VirtualCamera.m_Lens.OrthographicSize -= Time.deltaTime * 4;
            }
            else
            {
                VirtualCamera.m_Lens.OrthographicSize = 3f;
            }
        }
        else
        {
            if (VirtualCamera.m_Lens.OrthographicSize < 5)
            {
                VirtualCamera.m_Lens.OrthographicSize += Time.deltaTime * 4;
            }
            else
            {
                VirtualCamera.m_Lens.OrthographicSize = 5f;
            }
        }

    }

    public void OnDamage()
    {
        CurHealth--;
        if (CurHealth <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            MoveSpeed /= 2;
            Invoke("OffDamage", 0.5f);
            VirtualCamera.m_Lens.OrthographicSize = 4.8f;
        }
    }

    void OffDamage()
    {
        ZoomIn = false;
        MoveSpeed *= 2;
    }
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, JumpForce);
        IsJump = true;
        CanJump = false;
        canCheckGround = false; // 일정 시간 동안 바닥 감지 방지
        StartCoroutine(EnableGroundCheckAfterDelay());
    }

    IEnumerator EnableGroundCheckAfterDelay()
    {
        yield return new WaitForSeconds(jumpBufferTime);
        canCheckGround = true; // 일정 시간 후 바닥 체크 활성화
    }

    IEnumerator StickEffect()
    {
        VirtualCamera.m_Lens.OrthographicSize = 4.7f;
        yield return new WaitForSeconds(0.4f);
        ZoomIn = false;
    }

    IEnumerator Dash()
    {

        Vector2 startPos = transform.position;
        Vector2 targetPos = CrossHair.transform.position;
        Vector2 dashDirection = (targetPos - startPos).normalized;

        RaycastHit2D hit = Physics2D.Raycast(startPos, dashDirection, Vector2.Distance(startPos, targetPos), WallLayer);
        RaycastHit2D Enemyhit = Physics2D.Raycast(startPos, dashDirection, Vector2.Distance(startPos, targetPos), LayerMask.GetMask("HitLayer"));


        if (hit.collider != null)
        {
            targetPos = hit.point - dashDirection * 0.5f;
        }

        if (Enemyhit.collider != null)
        {
            if (Enemyhit.collider.CompareTag("Enemy"))
            {
                if (IsDashing)
                {
                    ZoomIn = true;
                    Transform transform = Enemyhit.collider.transform;

                    bool AttackLeftOrRight(Transform enemy)
                    {
                        return enemy.position.x < gameObject.transform.position.x;
                    }

                    Enemy enemyHp = Enemyhit.collider.gameObject.GetComponent<Enemy>();
                    if (!enemyHp.Mujuck)
                    {
                        StartCoroutine(enemyHp.BackMove(AttackLeftOrRight(transform)));
                        enemyHp.OnDamage();
                    }
                }
            }
            else if (Enemyhit.collider.CompareTag("Miniboss"))
            {
                if (IsDashing)
                {
                    ZoomIn = true;
                    Transform transform = Enemyhit.collider.transform;

                    bool AttackLeftOrRight(Transform enemy)
                    {
                        return enemy.position.x < gameObject.transform.position.x;
                    }

                    ShortAttackMiniBoss enemyHp = Enemyhit.collider.gameObject.GetComponent<ShortAttackMiniBoss>();
                    if (!enemyHp.Mujuck)
                    {
                        StartCoroutine(enemyHp.BackMove(AttackLeftOrRight(transform)));
                        enemyHp.OnDamage();
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.01f);
        gameObject.layer = LayerMask.NameToLayer(dashLayerName);
        rb.velocity = dashDirection * DashSpeed;
        yield return new WaitForSeconds(DashTime);

        gameObject.layer = defaultLayer;
        OffZoom();
        CanDash = false;
        Effect.transform.DOScale(0, 0.1f);
        rb.velocity = Vector2.zero;
        IsDashing = false;
        MoveSpeed = 5;
        yield return new WaitForSeconds(1.5f);
        CanDash = true;
    }

    void OffZoom()
    {
        ZoomIn = false;
    }

    void JumpCheck()
    {
        if (!canCheckGround) return;

        RaycastHit2D hitL = Physics2D.Raycast(transform.position, Vector2.down + Vector2.left * 0.2f, 0.5f, WallLayer);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, WallLayer);
        RaycastHit2D hitR = Physics2D.Raycast(transform.position, Vector2.down + Vector2.right * 0.2f, 0.5f, WallLayer);

        Debug.DrawRay(transform.position + Vector3.left * 0.2f, Vector2.down * 0.5f, Color.red);
        Debug.DrawRay(transform.position, Vector2.down * 0.5f, Color.red);
        Debug.DrawRay(transform.position + Vector3.right * 0.2f, Vector2.down * 0.5f, Color.red);

        if (hit.collider || hitL.collider || hitR.collider)
        {
            IsJump = false;
            CanJump = true;
            rb.gravityScale = 1f;
            Effect.transform.DOScale(0, 0.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //StopCoroutine(Dash());
            //CanDash = true;

            //rb.gravityScale = 1f;
            //PlayerMaxYPos = Vector2.zero;
            //Effect.transform.DOScale(0, 0.1f);
            //IsJump = false;
            //CanJump = true;
            //MoveSpeed = 5;
        }
    }
}
