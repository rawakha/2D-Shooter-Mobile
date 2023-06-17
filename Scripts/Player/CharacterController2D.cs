using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    //animations
    public enum State
    {
        Normal,
        Rolling,
    }

    [Header("Animations")]
    [SerializeField] private Animator animator;

    //movement
    private Rigidbody2D rb;
    private PlayerScript playerScript;
    private Vector3 moveDirection;
    private Vector3 rollDirection;
    private State state;

    [Header("Movement Stats")]
    [SerializeField] public float moveSpeed;
    [SerializeField] public float rollingSpeedStart;

    [HideInInspector] public float currentMoveSpeed;
    private float rollingSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerScript = GetComponent<PlayerScript>();
        state = State.Normal;

        currentMoveSpeed = moveSpeed;
        rollingSpeed = rollingSpeedStart;
    }

    private void Update()
    {
        switch (state) 
        {
            case State.Normal:
                float moveX = 0f;
                float moveY = 0f;

                if (Input.GetKey(KeyCode.W))
                {
                    moveY = 1f;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    moveY = -1f;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    moveX = -1f;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    moveX = 1f;
                }

                moveDirection = new Vector3(moveX, moveY).normalized;

                if (playerScript.dead == false)
                {
                    //run animation
                    if (moveDirection != Vector3.zero)
                    {
                        animator.SetBool("Moving", true);
                    }
                    else
                    {
                        animator.SetBool("Moving", false);
                    }

                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        rollDirection = moveDirection;
                        rollingSpeed = rollingSpeedStart;
                        state = State.Rolling;
                        animator.SetBool("Rolling", true);
                    }
                }

                break;
            case State.Rolling:
                
                float rollingSpeedDropMultiplier = 1f;
                rollingSpeed -= rollingSpeed * rollingSpeedDropMultiplier * Time.deltaTime;

                float rollingSpeedMinimum = 5f;
                if (rollingSpeed < rollingSpeedMinimum) 
                {
                    state = State.Normal;
                    animator.SetBool("Rolling", false);
                }

                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Normal:
                if (playerScript.dead == false)
                {
                    rb.velocity = moveDirection * currentMoveSpeed;
                }
                break;
            case State.Rolling:
                if (playerScript.dead == false)
                {
                    rb.velocity = rollDirection * rollingSpeed;
                }
                break;
        }
        
    }
}

