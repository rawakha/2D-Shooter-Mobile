using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using BarthaSzabolcs.Tutorial_SpriteFlash;
using Pathfinding;

public class FlyingEnemy : MonoBehaviour
{
    [Header("PathFinding")]
    private Vector2 target;
    Seeker seeker;
    Path path;
    public float nextWayPointDistance = 0.1f;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    [Header("Stats")]
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private float cashToAdd;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float shootingRange;
    [SerializeField] private float retreatRange;
    [SerializeField] private float bulletSpeed;
    private float lastShotTime;

    [SerializeField] private LayerMask playerLayer;

    [Header("Effects")]
    [SerializeField] private ScreenShake screenShake;
    [SerializeField] private NoiseSettings enemyDeathShakeProfile;
    [SerializeField] private GameObject deathParticle;
    [SerializeField] private GameObject shadow;

    [Header("Screen Shake")]
    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeDuration;

    [Header("PopUps")]
    [SerializeField] private GameObject damagePopUp;

    [Header("Health")]
    [SerializeField] private float health;
    private bool isDead;
    private float currentHealth;

    [Header("Separation")]
    [SerializeField] private float separationDistance;
    [SerializeField] private float separationForce;

    private GameObject player;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider2D;
    private PlayerScript playerScript;
    private FlashEffect flashEffect;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        sr = GetComponentInChildren<SpriteRenderer>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        flashEffect = GetComponent<FlashEffect>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();

        currentHealth = health;

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void Update()
    {
        HandlePlayerInteraction();
        ApplySeparationForce();
    }

    private void HandlePlayerInteraction()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= shootingRange && Time.time - lastShotTime > attackCooldown)
        {
            ShootAtPlayer();
        }

        if (distanceToPlayer <= retreatRange)
        {
            RetreatFromPlayer(speed);
            CheckPath();
        }
        else
        {
            ChasePlayer();
            CheckPath();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();

        if (bullet != null)
        {
            if (!isDead)
            {
                HandleDeath();
                EnemyTakeDamage(bullet.bulletDamage);
                DisplayDamagePopup(bullet.bulletDamage);
            }
        }
    }

    //Pathfinding & Chase
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void CheckPath()
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }
    }

    private void ChasePlayer()
    {
        target = player.transform.position;
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 desiredVelocity = direction * speed * Time.deltaTime;
        rb.MovePosition(rb.position + desiredVelocity);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWayPointDistance)
        {
            currentWaypoint++;
        }

        FlipSprite(direction);
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target, OnPathComplete);
        }
    }

    private void ShootAtPlayer()
    {
        Debug.Log("Shoot");
        Vector2 direction = (player.transform.position - transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        lastShotTime = Time.time;
    }

    private void RetreatFromPlayer(float retreatSpeed)
    {
        if (player != null)
        {
            Vector2 direction = (transform.position - player.transform.position).normalized; // Note this is reversed compared to chasing

            // You might want to add some conditions here, for example, to only retreat when health is low

            Vector2 desiredVelocity = direction * retreatSpeed;
            rb.velocity = Vector2.Lerp(rb.velocity, desiredVelocity, Time.deltaTime);
        }
    }

    //damage and death
    private void DeathVisual()
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity);
    }

    public void EnemyTakeDamage(float damage)
    {
        flashEffect.Flash();
        currentHealth -= damage;
    }

    public void EnemyAddHealth(float healthToAdd)
    {
        currentHealth += healthToAdd;
    }

    private void HandleDeath()
    {
        if (currentHealth <= 0)
        {
            playerScript.AddCash(cashToAdd);

            isDead = true;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            circleCollider2D.enabled = false;

            DeathVisual();
            Destroy(gameObject);
            Destroy(shadow);
        }
    }

    //Seperation Force
    private void ApplySeparationForce()
    {
        Vector2 force = CalculateSeparationForce();

        if (force.magnitude > 0)
        {
            rb.AddForce(force * separationForce);
        }
    }

    private Vector2 CalculateSeparationForce()
    {
        Vector2 force = Vector2.zero;
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, separationDistance);

        foreach (Collider2D enemy in nearbyEnemies)
        {
            if (enemy.gameObject.CompareTag("Enemy") && enemy.gameObject != gameObject)
            {
                Vector2 direction = transform.position - enemy.transform.position;
                force += direction.normalized / direction.magnitude;
            }
        }

        return force;
    }

    //effects
    private void FlipSprite(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                sr.flipX = false;
            }
            else if (direction.x < 0)
            {
                sr.flipX = true;
            }
        }
    }

    private void DisplayDamagePopup(float damage)
    {
        GameObject damageText = Instantiate(damagePopUp, transform.position, Quaternion.identity) as GameObject;
        damageText.GetComponentInChildren<TextMeshPro>().text = "<" + damage.ToString() + ">";
    }
}
