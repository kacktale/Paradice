using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.LowLevel;

public class ShortAttackMiniBoss : MonoBehaviour
{
    [Header("스텟")]
    public int Hp;
    public float backForce;

    [Header("인식관련")]
    public bool Mujuck;
    public LayerMask LayerMask;
    public GameObject player;
    Player PlayerComp;

    [Header("이펙트&파티클")]
    public ParticleSystem DamageParticle;
    public enum EnemyState
    {
        Idle, Damage, Run, ShortAttack, Hide
    }
    public EnemyState CurrentState = EnemyState.Idle;

    [Header("슬로우 관련")]
    public float MoveSpeed;
    public float AttackSpeed = 1;
    private float ActTime = 3f;
    private float DMGOffTime = 0.3f;

    private SpriteRenderer Sprite;
    private Rigidbody2D Rigidbody;

    private bool IdleAnim = false;
    [SerializeField]
    private bool IsAttack = false;
    private bool IsHiding = false;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Rigidbody.velocity = Vector3.zero;
        Sprite = GetComponent<SpriteRenderer>();
        PlayerComp = player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
    }

    private void CheckState()
    {
        switch (CurrentState)
        {
            case EnemyState.Idle:
                IdleState();
                break;
            case EnemyState.Run:
                FollowPlayer();
                break;
            case EnemyState.Damage:
                break;
            case EnemyState.ShortAttack:
                break;
            case EnemyState.Hide:
                BackWardHide();
                Invoke("FollowPlayer", ActTime);
                break;
        }
    }
    void IdleState()
    {
        if (!IdleAnim)
        {
            IdleAnim = true;
            Invoke("HideState", ActTime);
        }
    }

    private void HideState()
    {
        CurrentState = EnemyState.Hide;
        Sprite.DOFade(0.1f, ActTime);
    }

    void BackWardHide()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        //플레이어가 적보다 오른쪽에 있을때
        if (player.transform.position.x > transform.position.x) transform.rotation = Quaternion.Euler(0, 180, 0);
        else transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position -= new Vector3(direction.x * MoveSpeed * 0.1f * Time.deltaTime, 0, 0);
    }

    public void FollowPlayer()
    {
        float distance = MathF.Abs(player.transform.position.x - transform.position.x);
        if (distance <= 1)
        {
            if (!IsAttack)
            {
                IsAttack = true;
                CurrentState = EnemyState.ShortAttack;
                StartCoroutine(ShortAttackAnim());
            }
        }
        else
        {
            if(!IsAttack)
            {
                CurrentState = EnemyState.Run;
                Vector2 direction = (player.transform.position - transform.position).normalized;
                //플레이어가 적보다 오른쪽에 있을때
                if (player.transform.position.x > transform.position.x)
                {
                    //Sprite.flipX = false;
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    transform.position += new Vector3(direction.x * MoveSpeed * Time.deltaTime, 0, 0);
                }
                //왼쪽에 있을때
                else if (player.transform.position.x < transform.position.x)
                {
                    //Sprite.flipX = true;
                    transform.rotation = Quaternion.Euler(0,0,0);
                    transform.position += new Vector3(direction.x * MoveSpeed * Time.deltaTime, 0, 0);
                }
            }
        }
    }

    IEnumerator ShortAttackAnim()
    {
        Sprite.DOFade(1f, ActTime/10);
        yield return new WaitForSeconds(AttackSpeed);
        if(Vector2.Distance(player.transform.position,transform.position) <= 1.2f)
        PlayerComp.OnDamage(2);
        CurrentState = EnemyState.Idle;
        yield return new WaitForSeconds(AttackSpeed *3);
        CurrentState = EnemyState.Hide;
        HideState();
        IsAttack = false;
    }

    public void OnDamage()
    {
        if(!Mujuck)
        {
            Hp--;
            if (Hp <= 0) Destroy(gameObject);
            Mujuck = true;
            Sprite.color = Color.red;
            CurrentState = EnemyState.Damage;
            Sprite.DOFade(0.5f, DMGOffTime);
            Invoke("OffDamage", DMGOffTime);
        }
    }

    private void OffDamage()
    {
        if(!IsAttack)   CurrentState = EnemyState.Run;
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Slow"))
        {
            MoveSpeed /= 2;
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
            DMGOffTime /= 2;
            ActTime /= 2;
            AttackSpeed /= 2;
        }
    }

    private void OnDestroy()
    {
        Sprite.color = new Color(0, 0, 0, 0);
    }
}
