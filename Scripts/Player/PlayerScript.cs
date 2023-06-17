using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using BarthaSzabolcs.Tutorial_SpriteFlash;

public class PlayerScript : MonoBehaviour
{
    //Shoot Event
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Vector3 gunEndPointPosition;
        public Vector3 shootPosition;
        public Vector3 shellPosition;
    }

    public Transform aimTransform;
    public Transform visualTransform;
    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rb2D;
    private FlashEffect flashEffect;

    [Header("Player Health")]
    [SerializeField] public float playerHealthStart;
    public float playerHealth;
    public bool dead;

    [Header("Cash")]
    public float currentCash;

    [Header("Wave")]
    public float waveCount;

    [Header("Animations")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Ui Animations")]
    [SerializeField] private Animator healthAnimator;
    [SerializeField] private Animator scoreAnimator;
    [SerializeField] private Animator waveAnimator;

    public bool playerIsShooting;
    private bool flipOnce;

    private void Awake()
    {
        aimTransform = transform.Find("Aim");
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        flashEffect = GetComponent<FlashEffect>();

        rb2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        currentCash = 0;
        waveCount = 0;
        playerHealth = playerHealthStart;

        flipOnce = false;
        boxCollider2D.enabled = true;
        dead = false;
    }

    private void Update()
    {
        HandleAiming();
        CheckShootingStatus();
        HandlePlayerHealth();
    }

    private void HandleAiming()
    {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle); // Add 180 degrees to the angle

        Vector3 weaponLocalScale = Vector3.one;
        Vector3 playerLocalScale = Vector3.one; 

        if (angle > 90 || angle < -90)
        {
            weaponLocalScale.x = -1f;
            weaponLocalScale.y = -1f;
            playerLocalScale.x = -1f;
        }
        else
        {
            weaponLocalScale.x = +1f;
            weaponLocalScale.y = +1f;
            playerLocalScale.x = +1f;
        }

        aimTransform.localScale = weaponLocalScale;
        transform.localScale = playerLocalScale;
    }

    public Vector2 GetAimingDirection()
    {
        // Get the mouse position in world coordinates
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();

        // Calculate the direction from the player to the mouse position
        Vector2 aimingDirection = (mousePosition - transform.position).normalized;

        return aimingDirection;
    }

    private void CheckShootingStatus()
    {
        playerIsShooting = Input.GetMouseButton(0);
    }

    //Health
    private void HandlePlayerHealth()
    {
        if (playerHealth <= 0f) 
        {
            dead = true;
            rb2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

            //player death
            Debug.Log("!!Player Death!!");
            DeathVisual();
        }
    }

    public void PlayerTakeDamage(float damage)
    {
        flashEffect.Flash();
        playerHealth -= damage;
    }

    private void DeathVisual()
    {
        boxCollider2D.enabled = false;

        //animations
        animator.SetTrigger("Death");

        Destroy(gameObject, 1f);
    }

    //Cash
    public void AddCash(float cashToAdd)
    {
        currentCash += cashToAdd;
        scoreAnimator.SetTrigger("CashIncrease");
    }

    public void RemoveCash(float cashToRemove)
    {
        currentCash -= cashToRemove;
        Debug.Log(cashToRemove);
    }
}
