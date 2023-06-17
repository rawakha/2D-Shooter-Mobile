using BarthaSzabolcs.Tutorial_SpriteFlash;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using Pathfinding;

public class EnemyAi : MonoBehaviour
{

    // Components
    private Rigidbody2D rb;
    private CapsuleCollider2D boxCollider2D;
    private FlashEffect flashEffect;
    private Animator animator;
    [SerializeField] private SpriteRenderer sr;

    // Player and spawner references
    private PlayerScript playerScript;

    [Header("PathFinding")]
    Seeker seeker;
    Path path;
    public float nextWayPointDistance = 0.1f;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    [Header("Player Pos")]
    [SerializeField] private Transform playerPosition;
    [SerializeField] private LayerMask playerLayer;

    [Header("Health")]
    [SerializeField] private float maxHealth;
    public float health;
    public bool isDead;
    public float currentHealth;

    // Stats
    [Header("Stats")]
    [SerializeField] public float attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float speed;
    [SerializeField] private float cashToAdd;
    private float lastAttackTime;

    // Visual effects
    [Header("Effects")]
    [SerializeField] private ScreenShake screenShake;
    [SerializeField] private NoiseSettings enemyDeathShakeProfile;
    [SerializeField] private GameObject deathParticle;
    [SerializeField] private GameObject shadow;

    // Popups
    [Header("PopUps")]
    [SerializeField] private GameObject damagePopUp;

    //retreat
    [Header("Retreat")]
    [SerializeField] private float separationDistance = 2f;

    // Screen shake on death
    [Header("Screen Shake")]
    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeDuration;

    private void Start()
    {
        InitializeStartingState();
        InitializeComponents();
        FindPlayerPosition();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    private void Update()
    {
        if (!isDead)
        {
            CheckPath();
            ChasePlayer();
            CheckForAttack();
            SeparateFromOthers();
        }

        HandleDeath();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();

        if (bullet != null)
        {
            if (!isDead)
            {
                Debug.Log("Damaged" + bullet.bulletDamage);
                EnemyTakeDamage(bullet.bulletDamage);
                DisplayDamagePopup(bullet.bulletDamage);
            }
        }
    }

    private void InitializeStartingState()
    {
        nextWayPointDistance = 0.1f;
        currentHealth = maxHealth;
        isDead = false;
    }

    private void InitializeComponents()
    {
        playerScript = FindObjectOfType<PlayerScript>();
        flashEffect = GetComponent<FlashEffect>();
        animator = GetComponentInChildren<Animator>();
        boxCollider2D = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        seeker = GetComponent<Seeker>();
        screenShake = FindAnyObjectByType<ScreenShake>();

        animator.SetBool("Dead", false);
        boxCollider2D.enabled = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void FindPlayerPosition()
    {
        if (playerPosition == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerPosition = player.transform;
            }
        }
    }

    private void HandleDeath()
    {
        if (currentHealth <= 0)
        {
            animator.SetTrigger("Death");
            Debug.Log("Enemy destroyed");
            playerScript.AddCash(cashToAdd);

            isDead = true;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            boxCollider2D.enabled = false;

            DeathVisual();
            Destroy(gameObject);
            Destroy(shadow);
        }
    }

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
            seeker.StartPath(rb.position, playerPosition.position, OnPathComplete);
        }
    }

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

    //attack
    private bool IsPlayerInRange()
    {
        return Vector2.Distance(transform.position, playerScript.transform.position) <= attackRange;
    }

    private void CheckForAttack()
    {
        if (Time.time > lastAttackTime + attackCooldown && IsPlayerInRange())
        {
            Debug.Log("MeleeAttack");
            MeleeAttack();
            lastAttackTime = Time.time;
        }
    }

    private void MeleeAttack()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);

        foreach (Collider2D playerCollider in hitPlayer)
        {
            playerScript.PlayerTakeDamage(attackDamage);
        }
    }

    private void SeparateFromOthers()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, separationDistance);
        foreach (var obj in nearbyObjects)
        {
            if (obj.gameObject != gameObject && obj.tag == "Enemy")
            {
                Vector2 directionToEnemy = transform.position - obj.transform.position;
                float distanceToEnemy = directionToEnemy.magnitude;

                // If too close to another enemy, apply a repelling force
                if (distanceToEnemy < separationDistance)
                {
                    // The repelling force is stronger the closer the enemies are
                    float forceMagnitude = (separationDistance / distanceToEnemy) - 1;
                    Vector2 force = directionToEnemy.normalized * forceMagnitude;

                    // Apply the force to both enemies
                    rb.AddForce(force);
                    Rigidbody2D enemyRb = obj.GetComponent<Rigidbody2D>();
                    if (enemyRb != null)
                    {
                        enemyRb.AddForce(-force);
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void DisplayDamagePopup(float damage)
    {
        GameObject damageText = Instantiate(damagePopUp, transform.position, Quaternion.identity) as GameObject;
        damageText.GetComponentInChildren<TextMeshPro>().text = "<" + damage.ToString() + ">";
    }
}


