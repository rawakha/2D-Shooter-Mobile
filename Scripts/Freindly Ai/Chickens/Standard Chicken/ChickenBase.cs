using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Pathfinding;

public class ChickenBase : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider2D;
    private Animator animator;
    private SpriteRenderer sr;

    [Header("PathFinding")]
    Seeker seeker;
    Path path;
    public float nextWayPointDistance = 0.1f;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;
    private Transform target;

    [Header("Stats")]
    public int maxHealth;
    private float health;
    public float speed;

    [Header("States")]
    public bool idle;
    public bool hungry;
    public bool eggReady;
    public bool itemFound;
    public bool itemAcquired;

    private void Start()
    {
        InitializeComponents();
        idle = true;

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    private void Update()
    {
        CheckPath();
        MoveToTarget();

        if(idle)
        {
            SetTargetRandom();
        }
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        animator = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    //Movement
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

    private void MoveToTarget()
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

    private void SetTargetRandom()
    {
        // Set the radius within which the chicken should wander
        float wanderRadius = 5.0f;

        // Calculate a random position within the radius
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        Vector3 targetPosition = new Vector3(transform.position.x + randomDirection.x, transform.position.y + randomDirection.y, transform.position.z);

        // Update the target
        if (idle)
        {
            GameObject targetObject = new GameObject("Target");
            target = targetObject.transform;
        }
        target.position = targetPosition;

        // Request a path update
        UpdatePath();
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
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
}
