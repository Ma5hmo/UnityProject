using UnityEngine;

public class GunPickUp : MonoBehaviour
{
    WeaponManager weaponManager;

    public Weapon wpn;
    public int ammoOnWeapon;
    Collider2D thisCollider;
    Rigidbody2D rb;
    [HideInInspector]
    public bool isTossed;

    private void Start()
    {
        thisCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        weaponManager = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponManager>();
        if(!isTossed)
            ammoOnWeapon = wpn.maxAmmoCount;
    }
    private void Update()
    {
        Debug.Log(weaponManager.isInventoryFull);

        Collider2D playerCol = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        if (weaponManager.isInventoryFull && rb.IsTouching(playerCol))
        {
            if (Input.GetKey(KeyCode.E))
            {
                weaponManager.WeaponPickUp(wpn, ammoOnWeapon);
                Destroy(gameObject);
            }
        }
        if (rb.velocity.sqrMagnitude < .1  && rb.angularVelocity < .1)
        {
            thisCollider.isTrigger = true;
        }
        else
        {
            thisCollider.isTrigger = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(rb.IsSleeping())
        {
            if (col.gameObject.CompareTag("Player"))
            {
                //Physics2D.IgnoreCollision(col.gameObject.GetComponent<Collider2D>(), thisCollider);
                if (!weaponManager.isInventoryFull)
                {
                    weaponManager.WeaponPickUp(wpn, ammoOnWeapon);
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }
    /*/
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

        }
    }
    /*/
}