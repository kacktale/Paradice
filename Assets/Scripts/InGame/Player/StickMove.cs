using UnityEngine;

public class StickMove : MonoBehaviour
{
    public Transform TargetPos;
    public float Speed;
    public float BounceForce;
    public float collectDistance;
    public Transform player;
    public LayerMask WallLayer; // 벽 감지를 위한 레이어 추가

    private Vector3 ShootPos;
    private Vector3 moveDirection;
    private Rigidbody2D rb;
    private bool IsGrab = false;
    private bool isNearWall = false;
    private bool Shoot = false;

    [SerializeField] float bounceForce = 10f;
    [SerializeField] float bounceRandomness = 10f;
    [SerializeField] float bounceDamp = 0.9f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        ShootPos = TargetPos.position;
        moveDirection = (ShootPos - transform.position).normalized;

        isNearWall = Physics2D.Raycast(player.position, moveDirection, 0.6f, WallLayer);

        if (isNearWall)
        {
            transform.position = player.position + (Vector3)moveDirection * 0.6f;
        }
        else
        {
            transform.position += moveDirection * 0.5f;
        }

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, moveDirection, 0.3f, WallLayer);
        Debug.DrawRay(transform.position, ShootPos);
        if (wallHit.collider != null)
        {
            StickToWall(wallHit.point);
        }
        if(!Shoot)  rb.velocity = moveDirection * Speed;    Shoot = false;
        if (IsGrab)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= collectDistance)
            {
                Player stick = player.GetComponent<Player>();
                stick.RemainStick++;
                Destroy(gameObject);
            }
        }

    }

    private void StickToWall(Vector2 hitPoint)
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;

        transform.position = hitPoint - (Vector2)moveDirection * 0.2f;

        moveDirection = Vector3.zero;
        IsGrab = true;
    }

    void PhysicReload(GameObject other)
    {
        Vector2 contactDir = (transform.position - other.transform.position).normalized;

        moveDirection = Vector2.Reflect(moveDirection, contactDir).normalized;

        float randomAngle = Random.Range(-bounceRandomness, bounceRandomness);
        moveDirection = Quaternion.Euler(0, 0, randomAngle) * moveDirection;

        if (moveDirection.y > 0.2f)
        {
            moveDirection.y = Random.Range(-0.4f, -0.2f);
            moveDirection = moveDirection.normalized;
        }

        float adjustedForce = bounceForce * bounceDamp;
        rb.velocity = moveDirection * adjustedForce;

        Speed /= 3;

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, 1f, WallLayer);
            if (hit.collider != null)
            {
                StickToWall(hit.point);
            }
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!IsGrab)
            {
                PhysicReload(collision.gameObject);
                Enemy enemyHp = collision.gameObject.GetComponent<Enemy>();
                if (!enemyHp.Mujuck)
                {
                    enemyHp.OnDamage();
                }
            }
        }
        if (collision.gameObject.CompareTag("Miniboss"))
        {
            if (!IsGrab)
            {
                PhysicReload(collision.gameObject);
                ShortAttackMiniBoss enemyHp = collision.gameObject.GetComponent<ShortAttackMiniBoss>();
                if (!enemyHp.Mujuck)
                {
                    enemyHp.OnDamage();
                }
            }
        }
        if (collision.gameObject.CompareTag("MiniBossLong"))
        {
            if (!IsGrab)
            {
                PhysicReload(collision.gameObject);
                LongAttackMiniboss enemyHp = collision.gameObject.GetComponent<LongAttackMiniboss>();
                if (!enemyHp.Mujuck)
                {
                    enemyHp.OnDamage();
                }
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (IsGrab)
            {
                Player stick = collision.gameObject.GetComponent<Player>();
                stick.RemainStick++;
                Destroy(gameObject);
            }
        }
    }
}
