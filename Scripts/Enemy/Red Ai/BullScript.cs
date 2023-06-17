using BarthaSzabolcs.Tutorial_SpriteFlash;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using Pathfinding;

public class BullScript : MonoBehaviour
{
    // Components
    private Rigidbody2D rb;
    private CapsuleCollider2D boxCollider2D;
    private FlashEffect flashEffect;
    private Animator animator;
    private SpriteRenderer sr;

    [Header("PathFinding")]
    Seeker seeker;
    Path path;
    public float nextWayPointDistance = 0.1f;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    [Header("Building Pos")]
    private GameObject target;
    [SerializeField] private LayerMask buildingLayer;

    [Header("Health")]
    [SerializeField] private float maxHealth;
    public float currentHealth;
    public bool isDead;

    // Stats
    [Header("Stats")]
    [SerializeField] public float attackDamage;
    [SerializeField] private float targetRange;
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
    [SerializeField] private float retreatDistance = 10f;
    [SerializeField] private float wanderDistance = 5f;

    // Screen shake on death
    [Header("Screen Shake")]
    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeDuration;

    private void Start()
    {
        InitializeStartingState();
        InitializeComponents();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    private void Update()
    {
        if (!isDead)
        {
            target = FindClosestBuilding();

            if (target != null)
            {
                ChaseBuilding();
                CheckPath();

                if (Time.time > lastAttackTime + attackCooldown && IsBuildingInRange())
                {
                    MeleeAttack();
                    lastAttackTime = Time.time;
                }
            }

            SeparateFromOthers();
        }

        HandleDeath();
    }

    private GameObject FindClosestBuilding()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject building in buildings)
        {
            float distance = Vector3.Distance(transform.position, building.transform.position);
            if (distance < closestDistance)
            {
                closest = building;
                closestDistance = distance;
            }
        }

        return closest;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();

        if (bullet != null)
        {
            if (!isDead)
            {
                EnemyTakeDamage(bullet.bulletDamage);
                DisplayDamagePopup(bullet.bulletDamage);
            }
        }
    }

    private void InitializeStartingState()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    private void InitializeComponents()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        flashEffect = GetComponent<FlashEffect>();
        animator = GetComponentInChildren<Animator>();
        boxCollider2D = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        screenShake = FindAnyObjectByType<ScreenShake>();
        seeker = GetComponent<Seeker>();

        animator.SetBool("Dead", false);
        boxCollider2D.enabled = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void HandleDeath()
    {
        if (currentHealth <= 0)
        {
            Debug.Log("Enemy destroyed");

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

    private void ChaseBuilding()
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
            seeker.StartPath(rb.position, target.transform.position, OnPathComplete);
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

    private bool IsBuildingInRange()
    {
        Collider2D[] hitBuildings = Physics2D.OverlapCircleAll(transform.position, attackRange, buildingLayer);
        return hitBuildings.Length > 0;
    }

    private void MeleeAttack()
    {
        Collider2D[] hitBuildings = Physics2D.OverlapCircleAll(transform.position, attackRange, buildingLayer);

        foreach (Collider2D buildingCollider in hitBuildings)
        {
            BuildingHealth building = buildingCollider.gameObject.GetComponent<BuildingHealth>();
            if (building != null)
            {
                building.TakeDamage((int)attackDamage);  // Call the TakeDamage method from the BuildingHealth script
            }
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

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, targetRange);
    }

    private void DisplayDamagePopup(float damage)
    {
        GameObject damageText = Instantiate(damagePopUp, transform.position, Quaternion.identity) as GameObject;
        damageText.GetComponentInChildren<TextMeshPro>().text = "<" + damage.ToString() + ">";
    }
}
