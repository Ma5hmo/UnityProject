using UnityEngine;

public class Bullet : MonoBehaviour
{
    Gun gun;
    void Start()
    {
        gun = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponManager>().activeWeapon;
        GetComponent<Rigidbody2D>().velocity = transform.right * gun.bulletSpeed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().enemyTakeDamage(gun.damage);
        }
        if (collision.CompareTag("Player") == false && collision.CompareTag("Bullet") == false && collision.CompareTag("GroundWeapon") == false)
        {
            Destroy(gameObject);

        }
    }
    void OnBecameInvisible() //if i dont see the bullet anymore
    {
        Destroy(gameObject, 1f);
    }
}
