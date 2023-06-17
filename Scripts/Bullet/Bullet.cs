using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{ 
    public float speed = 20f;
    public float lifeTime = 5f;
    public float bulletDamage;

    private Rigidbody2D rb;
    private Vector2 target;

   public void SetBulletStats(float setDamage, float setSpeed, float setLifeTime)
   {
        bulletDamage = setDamage;
        speed = setSpeed;
        lifeTime = setLifeTime;
   }

   public void InitializeBullet(Vector2 targetPosition)
   {
       rb = GetComponent<Rigidbody2D>();
       target = targetPosition;

       Vector2 direction = (target - (Vector2)transform.position).normalized;
       rb.velocity = direction * speed;

       // Calculate the angle in radians and then convert it to degrees
       float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

       // Adjust the angle based on the orientation of your sprite. 
       // This example assumes the sprite is facing right in the source image.
       // If your sprite is facing upwards, you might need to subtract 90 degrees:
       angle -= 90;

       // Apply the rotation to the bullet's Rigidbody
       rb.rotation = angle;

       StartCoroutine(DestroyBulletAfterTime());
   }



    private IEnumerator DestroyBulletAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
