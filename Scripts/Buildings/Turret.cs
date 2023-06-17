using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject bulletPrefab;

    [Header("Stats")]
    public float fireRate;
    public float attackRadius;
    public float bulletSpeed;
    public float damage;
    public float bulletLifeTime; //range of bullet

    private GameObject target;
    private float lastShotTime;

    private PlayerScript player;

    private void Start()
    {
        player = FindObjectOfType<PlayerScript>();
    }

    void Update()
    {
        target = FindClosestEnemy();

        if (target != null)
        {
            if (Time.time - lastShotTime > 1 / fireRate)
            { 
                ShootAtTarget();
            }
        }
    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject closestEnemy = null;
        float closestDistance = attackRadius;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    void ShootAtTarget()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().InitializeBullet(target.transform.position);
        bullet.GetComponent<Bullet>().SetBulletStats(damage, bulletSpeed, bulletLifeTime);

        lastShotTime = Time.time;
    }
}
