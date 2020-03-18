using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }
    void ChangeWeaponSlot(Sprite weaponImage)
    {
        image.sprite = weaponImage;
    }
}
