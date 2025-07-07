using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("플레이어 스탯")]
    public float MoveSpeed;
    private float MaxSpeed;
    public float JumpForce;
    public bool IsJump = false;
    public float MaxHealth;
    public float CurHealth;
    public bool Mujuck = false;

    [Header("무기")]
    public GameObject Effect;
    public GameObject Stick;
    public CrossHair CrossHair;
    public int RemainStick;

    [Header("점프 관련")]
    public float jumpBufferTime = 0.1f;
    private bool canCheckGround = true;

    [Header("대쉬")]
    public bool CanDash = true;
    private bool IsDashing;
    public float DashSpeed = 20f;
    public float DashTime = 0.2f;
    public float DashCool;
    public LayerMask WallLayer;

    [Header("이펙트")]
    public CinemachineVirtualCamera VirtualCamera;
    private bool ZoomIn;

    private Rigidbody2D rb;
    private bool CanJump = false;

    private Vector2 PlayerMaxYPos;
    private int defaultLayer;
    public string dashLayerName = "DashOnly";
    public Image Transition;
    public Slider DashCoolUI;
    public TextMeshProUGUI DashTXT;
    private bool ChargingAnim = false;

    public GameObject IndicatorObj;
    public RectTransform TashUI;

    public GameObject PausePanel;
    public Slider hpBar;
    private Animator anim;
    void Awake()
    {
        CurHealth = MaxHealth;

        rb = GetComponent<Rigidbody2D>();

        RemainStick = 2;

        defaultLayer = gameObject.layer;

        DashCoolUI.maxValue = DashCool;
        DashCoolUI.value = 0;

        hpBar.maxValue = MaxHealth;
        hpBar.value = CurHealth;

        MaxSpeed = MoveSpeed;

        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        Invoke("TashUIHide", 2);
        FadeOut();
        DashCoolUI.gameObject.transform.DOMoveX(-1000, 0);
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
            if (h < 0)
            {
                transform.rotation = Quaternion.Euler(0, -180, 0);
            }
            else if (h > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            anim.SetBool("IsWalk", IsKeyInput(h));
            rb.velocity = new Vector2(h * MoveSpeed, rb.velocity.y);
        }
        hpBar.value = CurHealth;
    }

    bool IsKeyInput(float h)
    {
        if (h == 0) return false;
        else return true;
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
        if (Input.GetMouseButtonDown(0) && MoveSpeed != 0 && Time.timeScale == 1)
        {
            if (RemainStick > 0)
            {
                GameObject stick;
                StickMove StickCs;
                StickIndicator Indicator;
                if (!CanJump)
                {
                    Vector3 CheckRotation = transform.rotation == Quaternion.Euler(0, 0, 0) ? Vector3.left : Vector3.right;
                    stick = Instantiate(Stick, transform.position + CheckRotation, Quaternion.identity);
                }
                else  stick = Instantiate(Stick, transform.position, Quaternion.identity);

                StickCs = stick.GetComponent<StickMove>();
                StickCs.TargetPos = CrossHair.gameObject.transform;
                StickCs.player = gameObject.transform;

                Indicator = stick.GetComponent<StickIndicator>();
                Indicator.Target = stick;
                Indicator.Indicator = IndicatorObj;

                StartCoroutine(StickEffect());
                RemainStick--;
            }
        }
        //대쉬
        if (Input.GetMouseButtonDown(1) && CanDash && Time.timeScale == 1)
        {
            CanDash = false;
            IsDashing = true;
            StartCoroutine(Dash());
            StopCoroutine(ShowDashCool());
            StartCoroutine(ShowDashCool());
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!PausePanel.active)
            {
                PausePanel.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                PausePanel.SetActive(false);
                Time.timeScale = 1;
            }
        }

        if (RemainStick == 2) IndicatorObj.SetActive(false);
    }
    public void OnDamage(float Damage)
    {
        if (Mujuck) return;
        Mujuck = true;
        CurHealth -= Damage;
        if (CurHealth <= 0)
        {
            OnDeath();
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
        Mujuck = false;
        ZoomIn = false;
        MoveSpeed = MaxSpeed;
    }
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, JumpForce);
        IsJump = true;
        CanJump = false;
        canCheckGround = false;
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
        yield return new WaitForSeconds(DashCool);
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
    void OnDeath()
    {
        MoveSpeed = 0f;
        rb.gravityScale = 0f;
        CanJump = false;
        ZoomIn = true;
        CanDash = false;
        FadeIN(0);
    }
    void FadeIN(int Rooom)
    {
        Transition.DOFade(1, 1);
        NextRoom(Rooom);
    }
    void NextRoom(int a)
    {
        if (a == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene(a - 1);
        }
    }
    void FadeOut()
    {
        Transition.DOFade(0, 1);
    }

    void TashUIHide()
    {
        TashUI.DOAnchorPos(new Vector2(517, 396), 2);
    }
    IEnumerator ShowDashCool()
    {
        RectTransform pos = DashCoolUI.gameObject.GetComponent<RectTransform>();
        DashTXT.text = "Charging...";
        DashCoolUI.value = 0;
        DashCoolUI.DOValue(DashCool, DashCool);
        pos.DOAnchorPos3DX(-808.3f, 1);
        yield return new WaitForSeconds(DashCool);
        ChargingAnim = true;
        DashTXT.text = "Charged!";
        yield return new WaitForSeconds(DashCool + 0.5f);
        if (ChargingAnim)
            pos.DOAnchorPos3DX(-1400, 1);
        yield return new WaitForSeconds(1);
        ChargingAnim = false;
    }
}
