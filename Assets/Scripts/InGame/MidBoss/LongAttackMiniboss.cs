using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class LongAttackMiniboss : MonoBehaviour
{
    [Header("스텟")]
    public int Hp;
    public float backForce;
    public bool LongAttack;

    [Header("인식관련")]
    public bool Mujuck;
    public LayerMask LayerMask;
    private bool EdgeL = false;
    public LayerMask MoveLayerMask;
    public GameObject Player;
    private bool FoundPlayer = false;
    private float MoveVertical = 0;


    [Header("이펙트&파티클")]
    public ParticleSystem DamageParticle;
    public ParticleSystem DeathParticle;
    public enum EnemyState
    {
        Idle, Walk, Damage, Run, LongAttack, Charge
    }
    public EnemyState CurrentState = EnemyState.Idle;

    [Header("슬로우 관련")]
    public float MoveSpeed;
    public float AttackSpeed = 1;
    private float BulletSpawnTime = 0.0f;
    private float ActTime = 3f;
    private float DMGOffTime = 0.3f;
    private float CurSpawnTime;

    public EnemyFinder FoundEnemy;
    public GameObject Bullet;

    private SpriteRenderer Sprite;
    private Rigidbody2D Rigidbody;

    private bool ChargeAnim = false;
    private bool IsAttack = false;
    private bool LazerAnim = false;
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
        if(MoveVertical < -1)
        {
            MoveVertical += Time.deltaTime/10;
        }
        else if(MoveVertical > 1) 
        {
            MoveVertical -= Time.deltaTime/10;
        }
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
                break;
            case EnemyState.Run:
                FollowPlayer();
                Debug.Log("Run");
                break;
            case EnemyState.Damage:
                FoundPlayer = true;
                FollowPlayer();
                Debug.Log("Damage");
                break;
            case EnemyState.LongAttack:
                Shooting();
                if (!LazerAnim)
                {
                    LazerAnim = true;
                    ChargeAnim = false;
                    Invoke("StopAttack", 4);
                }
                break;
            case EnemyState.Charge:
                if (!ChargeAnim)
                {
                    ChargeAnim = true;
                    LazerAnim = false;
                    Invoke("Shooting",2);
                }
            break;
        }
    }
    //idle 일시 애니메이션 재생후 다시 걷기
    void IdleState()
    {
        CurrentState = EnemyState.Idle;
        Invoke("FollowPlayer", ActTime);
    }

    public void FollowPlayer()
    {
        float distance = MathF.Abs(Player.transform.position.x - transform.position.x);
        if (distance <= 1)
        {
            if (!IsAttack)
            {
                IsAttack = true;
                StayAwayFromPlayer();
            }
        }
        else if (distance <= 5)
        {
            CurrentState = EnemyState.Charge;
        }
        else
        {
            Vector2 direction = (Player.transform.position - transform.position).normalized;
            //플레이어가 적보다 오른쪽에 있을때
            if (Player.transform.position.x > transform.position.x)
            {
                Sprite.flipX = true;
                transform.position += new Vector3(direction.x, 0, 0) * MoveSpeed * Time.deltaTime;
            }
            //왼쪽에 있을때
            else
            {
                Sprite.flipX = false;
                transform.position += new Vector3(direction.x, 0, 0) * MoveSpeed * Time.deltaTime;
            }
        }
    }

    public void StopAttack()
    {
        CurrentState = EnemyState.Idle;
    }

    void StayAwayFromPlayer()
    {
        float PlayerPos = Player.transform.position.y;
        Vector3 NextMovePos = Vector3.zero;

        int a = Random.Range(0, 1);

        if (a == 1)
        {
            NextMovePos.x = PlayerPos + 2;
            Sprite.flipX = false;
        }
        else
        {
            NextMovePos.x = PlayerPos - 2;
            Sprite.flipX = true;
        }
        transform.position = Vector3.MoveTowards(transform.position, NextMovePos, Time.deltaTime * MoveSpeed);
    }

    public void OnDamage()
    {
        //CurrentState = EnemyState.Damage;
        Hp--;
        if (Hp <= 0) Destroy(gameObject);
        Mujuck = true;
        Sprite.color = Color.red;
        Sprite.DOFade(0.5f, DMGOffTime);
        Invoke("OffDamage", DMGOffTime);
    }

    private void OffDamage()
    {
        //if (FoundPlayer) CurrentState = EnemyState.Run;
        //else CurrentState = EnemyState.Idle;
        Mujuck = false;
        Sprite.color = Color.white;
        Sprite.DOFade(1f, DMGOffTime);
    }

    private void Shooting()
    {
        if (CurSpawnTime <= 0)
        {
            CurrentState = EnemyState.LongAttack;
            CurSpawnTime = BulletSpawnTime;
            GameObject OneBullet = Instantiate(Bullet, transform.position, Quaternion.identity);
            Bullet TargetSc = OneBullet.GetComponent<Bullet>();
            TargetSc.Recoil = false;
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

    private void OnDestroy()
    {
        Sprite.color = new Color(0, 0, 0, 0);
        ParticleSystem particle = Instantiate(DeathParticle);
        particle.gameObject.transform.position = transform.position;
        particle.Play();
    }
}
