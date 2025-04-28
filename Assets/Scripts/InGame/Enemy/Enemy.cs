using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("스텟")]
    public int Hp;
    public float backForce;
    public bool LongAttack;

    [Header("인식관련")]
    public bool Mujuck;
    public LayerMask LayerMask;
    public bool EdgeL = false;
    public LayerMask MoveLayerMask;
    public GameObject Player;
    private bool FoundPlayer = false;

    [Header("이펙트&파티클")]
    public ParticleSystem DamageParticle;
    public ParticleSystem DeathParticle;
    public enum EnemyState
    {
        Idle, Walk, Damage, Run, ShortAttack, LongAttack
    }
    public EnemyState CurrentState = EnemyState.Idle;

    [Header("슬로우 관련")]
    public float MoveSpeed;
    public float AttackSpeed = 1;
    private float BulletSpawnTime = 0.1f;
    private float ActTime = 3f;
    private float DMGOffTime = 0.3f;
    private float CurSpawnTime;

    public EnemyFinder FoundEnemy;
    public GameObject Bullet;

    private SpriteRenderer Sprite;
    private Rigidbody2D Rigidbody;

    private bool IdleAnim = false;
    private bool IsAttack = false;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Sprite = GetComponent<SpriteRenderer>();
        CurSpawnTime = BulletSpawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
        CurSpawnTime -= Time.deltaTime;
    }

    private void CheckState()
    {
        switch (CurrentState)
        {
            case EnemyState.Idle:
                IdleState();
                break;
            case EnemyState.Walk:
                Debug.Log("Walk");
                WalkState();
                FindPlayer();
                break;
            case EnemyState.Run:
                FollowPlayer();
                Debug.Log("Run");
                break;
            case EnemyState.Damage:
                FoundPlayer = true;
                FoundEnemy.FoundPlayer = true;
                FollowPlayer();
                Debug.Log("Damage");
                break;
            case EnemyState.ShortAttack:
                Debug.Log("ShortAttack");
                break;
            case EnemyState.LongAttack:
                Shooting();
                break;
        }
    }
    //idle 일시 애니메이션 재생후 다시 걷기
    void IdleState()
    {
        if (!IdleAnim && !LongAttack)
        {
            IdleAnim = true;
            Invoke("WalkState", ActTime);
        }
        if (FoundEnemy.FoundPlayer)
        {
            Invoke("FollowPlayer", ActTime);
        }
    }

    void WalkState()
    {
        IdleAnim = false;
        if (!FoundPlayer)
        {
            CurrentState = EnemyState.Walk;
            RaycastHit2D checkEdgeL = Physics2D.Raycast(transform.position, Vector2.left, 1, MoveLayerMask);
            Debug.DrawRay(transform.position + new Vector3(0, 0.5f), Vector2.left, Color.red);
            RaycastHit2D checkEdgeR = Physics2D.Raycast(transform.position, Vector2.right, 1, MoveLayerMask);
            Debug.DrawRay(transform.position + new Vector3(0, 0.5f), Vector2.right, Color.blue);

            Debug.Log(checkEdgeL.collider);
            Debug.Log(checkEdgeR.collider);

            if (checkEdgeL.collider != null && EdgeL && checkEdgeL.collider.gameObject.tag != "Enemy")
            {
                EdgeL = false;
                CurrentState = EnemyState.Idle;
                IdleState();
            }
            else if (checkEdgeR.collider != null && !EdgeL && checkEdgeL.collider.gameObject.tag != "Enemy")
            {
                EdgeL = true;
                CurrentState = EnemyState.Idle;
                IdleState();
            }
            else
            {
                Debug.Log("근처에 벽없음");
                if (!EdgeL)
                {
                    MoveRight();
                }
                else
                {
                    MoveLeft();
                }
            }
        }
    }

    void MoveLeft()
    {
        Sprite.flipX = false;
        transform.position -= new Vector3(MoveSpeed * Time.deltaTime, 0, 0);
    }

    void MoveRight()
    {
        Sprite.flipX = true;
        transform.position += new Vector3(MoveSpeed * Time.deltaTime, 0, 0);
    }

    public void FindPlayer()
    {
        RaycastHit2D PlayerFinderR = Physics2D.Raycast(transform.position, Vector2.right, 2f, LayerMask);
        Debug.DrawRay(transform.position, Vector2.right * 2f);
        RaycastHit2D PlayerFinderL = Physics2D.Raycast(transform.position, Vector2.left, 2f, LayerMask);
        Debug.DrawRay(transform.position, Vector2.left * 2f);

        if (PlayerFinderL.collider || PlayerFinderR.collider)
        {
            FoundPlayer = true;
            CurrentState = EnemyState.Run;
            FoundEnemy.FoundPlayer = true;
        }
    }

    public void FollowPlayer()
    {
        float distance = MathF.Abs(Player.transform.position.x - transform.position.x);
        if (distance <= 1)
        {
            if (!IsAttack)
            {
                IsAttack = true;
                if (!LongAttack)
                {
                    CurrentState = EnemyState.ShortAttack;
                    StartCoroutine(ShortAttackAnim());
                }
            }
        }
        else
        {
            if (!LongAttack)
            {
                Vector2 direction = (Player.transform.position - transform.position).normalized;
                //플레이어가 적보다 오른쪽에 있을때
                if (Player.transform.position.x > transform.position.x)
                {
                    Sprite.flipX = true;
                    transform.position += new Vector3(direction.x * MoveSpeed * Time.deltaTime, 0, 0);
                }
                //왼쪽에 있을때
                else
                {
                    Sprite.flipX = false;
                    transform.position += new Vector3(direction.x * MoveSpeed * Time.deltaTime, 0, 0);
                }
            }
            else
            {
                CurrentState = EnemyState.LongAttack;
            }
        }
    }

    IEnumerator ShortAttackAnim()
    {
        yield return new WaitForSeconds(AttackSpeed);
        Player player = Player.GetComponent<Player>();
        float distance = MathF.Abs(Player.transform.position.x - transform.position.x);
        if (distance <= 1)
        {
            player.OnDamage();
        }
        yield return new WaitForSeconds(AttackSpeed);
        CurrentState = EnemyState.Run;
        IsAttack = false;
    }

    public void OnDamage()
    {
        CurrentState = EnemyState.Damage;
        Hp--;
        if (Hp <= 0) Destroy(gameObject);
        Mujuck = true;
        Sprite.color = Color.red;
        Sprite.DOFade(0.5f, DMGOffTime);
        Invoke("OffDamage", DMGOffTime);
    }

    private void OffDamage()
    {
        if (FoundPlayer) CurrentState = EnemyState.Run;
        else CurrentState = EnemyState.Idle;
        Mujuck = false;
        Sprite.color = Color.white;
        Sprite.DOFade(1f, DMGOffTime);
    }

    public IEnumerator BackMove(bool Left)
    {
        if (Hp > 1)
        {
            if (Left)
            {
                Rigidbody.velocity = transform.position / 400 + Vector3.left * backForce;
                DamageParticle.gameObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                DamageParticle.Play();
                yield return new WaitForSeconds(0.1f);
                Rigidbody.velocity = Vector2.zero;
            }
            else
            {
                Rigidbody.velocity = transform.position / 400 - Vector3.left * backForce;
                DamageParticle.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                DamageParticle.Play();
                yield return new WaitForSeconds(0.1f);
                Rigidbody.velocity = Vector2.zero;
            }
        }
    }

    private void Shooting()
    {
        if (CurSpawnTime <= 0)
        {
            CurSpawnTime = BulletSpawnTime;
            GameObject OneBullet = Instantiate(Bullet, transform.position, Quaternion.identity);
            Bullet TargetSc = OneBullet.GetComponent<Bullet>();
            TargetSc.player = Player;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Slow"))
        {
            MoveSpeed /= 2;
            BulletSpawnTime *= 2;
            DMGOffTime *= 2;
            ActTime *= 2;
            AttackSpeed *= 2;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Slow"))
        {
            MoveSpeed *= 2;
            BulletSpawnTime /= 2;
            DMGOffTime /= 2;
            ActTime /= 2;
            AttackSpeed /= 2;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EdgeL = !EdgeL;
        }
    }

    private void OnDestroy()
    {
        Sprite.color = new Color(0, 0, 0, 0);
        ParticleSystem particle = Instantiate(DeathParticle);
        particle.gameObject.transform.position = transform.position;
        particle.Play();
    }
}
