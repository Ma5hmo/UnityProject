using UnityEngine;
using System;
public class GunPickUp : MonoBehaviour
{
    WeaponManager weaponManager;

    public string weaponName;
    public int ammoOnWeapon;

    [HideInInspector]
    public bool isTossed;

    private void Start()
    {
        weaponManager = GameObject.FindGameObjectWithTag("WeaponManager").GetComponent<WeaponManager>();

        if(!isTossed)
            ammoOnWeapon = Array.Find(weaponManager.guns, gun => gun.gunName == weaponName).maxAmmoCount;

        if (string.IsNullOrEmpty(weaponName)) { Debug.LogError("set the weapon name dude"); } //error message if no name on the weapon
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(GetComponent<Rigidbody2D>().IsSleeping())
        {
            if (col.CompareTag("Player"))
            {
                weaponManager.WeaponPickUp(weaponName, this);
                Destroy(gameObject);
            }
        }
    }
}