using UnityEngine;

[System.Serializable]
public class Gun
{

    public string gunName;
    public GameObject objectToThrow;
    public int maxAmmoCount;
    public float reloadTime;
    public float damage;
    public float fireRate;
    public float bulletSpeed;
    public Vector3 barrelPosition;
    public Sprite sprite;
    public Sprite WeaponSlotSprite;
    public Sprite WeaponSlotSpriteSelected;
    public bool isAutomatic;

}
