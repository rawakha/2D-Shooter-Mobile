using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] private float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerScript player = collision.gameObject.GetComponent<PlayerScript>();

        if(player != null)
        {
            Debug.Log("Damaged");
            player.PlayerTakeDamage(damage);
        }
    }
}
