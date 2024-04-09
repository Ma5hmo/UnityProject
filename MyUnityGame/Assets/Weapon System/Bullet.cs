using UnityEngine;

public class Bullet : MonoBehaviour
{
    Weapon gun;
    WeaponManager wpnManager;
    Rigidbody2D rb;

    void Awake()
    {
        wpnManager = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponManager>();
        gun = wpnManager.activeWeapon;
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        rb.velocity = transform.right * wpnManager.bulletSpeed;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            col.GetComponent<Enemy>().enemyTakeDamage(gun.damage);
            Destroy(gameObject);
        }
        if (!col.CompareTag("Player") && !col.CompareTag("Bullet")&& !col.CompareTag("GroundWeapon") && !col.CompareTag("Room"))
        {
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible() //if i dont see the bullet anymore
    {
        Destroy(gameObject, 1f);
    }
}
