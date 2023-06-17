using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerResources playerResources;
    public LayerMask playerLayer;

    [Header("Stats")]
    public float timeToGrow = 10.0f;  // Time in seconds it takes for the crop to grow
    public int harvestAmount = 2;
    public int plantCost = 1;
    public float harvestRadius;
    private float timer = 0.0f;

    private bool cropReady;
    private bool cropHarvested;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerResources = FindObjectOfType<PlayerResources>();
        animator = GetComponentInChildren<Animator>();

        cropReady = false;
        cropHarvested = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!cropReady)
        {
            HandleGrowth(); 
        }
    }

    private void OnMouseDown()
    {
        if(cropHarvested)
        {
            HandlePlanting();
        }

        HandleHarvest();
    }

    private void HandleGrowth()
    {
        timer += Time.deltaTime;

        if (timer > timeToGrow)
        {
            animator.SetTrigger("Grow");
            cropReady = true;
        }
    }

    private void HandleHarvest()
    {
        if (cropReady)
        {
            // Check if player is in radius
            Collider2D playerInRange = Physics2D.OverlapCircle(transform.position, harvestRadius, playerLayer);

            // If player is in radius and presses the 'E' key, harvest the crop
            if (playerInRange)
            {
                Harvest();
            }
        }
    }

    private void Harvest()
    {
        Debug.Log("Harvest");
        animator.SetTrigger("Harvest");
        cropHarvested = true;
        playerResources.AddCarrots(harvestAmount);
    }

    private void HandlePlanting()
    {
        if(cropHarvested)
        {
            // Check if player is in radius
            Collider2D playerInRange = Physics2D.OverlapCircle(transform.position, harvestRadius, playerLayer);

            // If player is in radius and presses the 'E' key, harvest the crop
            if (playerInRange)
            {
                Plant();
            }
        }
    }

    private void Plant()
    {
        if(playerResources.carrotAmount > plantCost)
        {
            //visual
            Debug.Log("Plant");
            animator.SetTrigger("Plant");

            //reset timer
            timer = 0f;

            //logic
            playerResources.RemoveCarrots(plantCost);
            cropHarvested = false;
            cropReady = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, harvestRadius);
    }
}
