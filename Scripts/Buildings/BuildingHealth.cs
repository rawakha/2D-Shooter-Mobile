using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth;
    public float currentHealth;
    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        active = true;
    }

    //health
    private void HandleHealth()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HandleHealth();
    }
}
