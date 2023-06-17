using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float attackDamage;
    [SerializeField] private float bulletLifetTime;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerScript playerScript = collision.gameObject.GetComponent<PlayerScript>();
            playerScript?.PlayerTakeDamage(attackDamage);
            Destroy(gameObject);
        }else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Destroy(gameObject, bulletLifetTime);
    }
}
