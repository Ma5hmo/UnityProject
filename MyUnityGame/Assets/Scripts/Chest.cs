using UnityEngine;


public class Chest : MonoBehaviour
{
    public ChestOpenAnim chestGFX;
    public WeaponManager weaponManager;
    public Weapon[] allWpns;
    
    Weapon chestWpn;

    public SpriteRenderer weaponRenderer;

    private void Start()
    {
        chestWpn = allWpns[Random.Range(0, allWpns.Length)];
        weaponRenderer.sprite = chestWpn.pickUpSprite;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (chestGFX.isOpen)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                weaponManager.WeaponPickUp(chestWpn, chestWpn.maxAmmoCount);
                Destroy(gameObject);
                
            }
        }
    }
}
