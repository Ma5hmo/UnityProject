using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public new string name;
    public bool isAutomatic;
    public float damage;
    public float fireRate;
    public int maxAmmoCount;
    public float reloadTime;
    public GameObject objectToThrow;
    public Vector2 barrelPosition;
    public Sprite sprite;
    public Sprite WeaponSlotSprite;
    public Sprite WeaponSlotSpriteSelected;
    public Sprite pickUpSprite;
}
