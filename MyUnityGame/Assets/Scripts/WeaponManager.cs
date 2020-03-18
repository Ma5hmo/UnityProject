using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    #region Variables
    public Gun[] guns;
    [HideInInspector]
    public Gun activeWeapon;

    readonly string[] weaponsHolding = new string[2];
    readonly int[] weaponsHoldingAmmo = new int[2];
    readonly GunPickUp[] weaponsHoldingScripts = new GunPickUp[2];

    private int activeWeaponNum = 0;

    public GameObject bulletPrefab;
    public GameObject barrelPrefab;
    public GameObject muzzleFlash;

    bool holdingWeapon;
    bool isReloading;

    public float weaponTossForce;

    public Text ammoText;
    public Slider reloadSlider;
    public Image weaponSlot1;
    public Image weaponSlot2;

    public Sprite emptyWeaponSlot;
    public Sprite emptySelectedWeaponSlot;

    private float nextFire = 0f;

    float reloadSliderTime;
    #endregion Variables

    #region Main Functions
    void Start()
    {
        ActiveWeaponUpdate();
    }

    void Update()
    {
        ChangeWeaponSlot();
        WeaponChange();
        IfThenReload(); //if(holdingWeapon) is located within the method
        if (holdingWeapon)
        {
            WeaponAim();
            IfThenShoot();
            if (Input.GetKeyDown(KeyCode.G))
                ThrowWeapon();
        }


        if (nextFire > 0.0f)
        {
            nextFire -= Time.deltaTime;
        }
        else if (nextFire < 0f)
        {
            nextFire = 0f;

        }
    }
    void LateUpdate()
    {
        ActiveWeaponUpdate();
    }
    #endregion MainFunctions

    #region Methods

    void ChangeWeaponSlot()
    {
        Gun weapon1 = Array.Find(guns, gun => gun.gunName == weaponsHolding[0]);
        Gun weapon2 = Array.Find(guns, gun => gun.gunName == weaponsHolding[1]);

        if (weaponsHolding[0] != null)
            weaponSlot1.sprite = weapon1.WeaponSlotSprite;
        else if(weaponsHolding[0] == null)
            weaponSlot1.sprite = emptyWeaponSlot;

        if (weaponsHolding[1] != null)
            weaponSlot2.sprite = weapon2.WeaponSlotSprite;
        else if (weaponsHolding[1] == null)
            weaponSlot2.sprite = emptyWeaponSlot;

        if (activeWeaponNum == 0 && weaponsHolding[0] != null)
            weaponSlot1.sprite = weapon1.WeaponSlotSpriteSelected;
        else if (activeWeaponNum == 0 && weaponsHolding[0] == null)
            weaponSlot1.sprite = emptySelectedWeaponSlot;

        if (activeWeaponNum == 1 && weaponsHolding[1] != null)
            weaponSlot2.sprite = weapon2.WeaponSlotSpriteSelected;
        else if (activeWeaponNum == 1 && weaponsHolding[1] == null)
            weaponSlot2.sprite = emptySelectedWeaponSlot;
    }

	void ThrowWeapon()
    {
        GameObject tossedWeapon = Instantiate(activeWeapon.objectToThrow, transform.position, Quaternion.identity);

        Rigidbody2D rb = tossedWeapon.GetComponent<Rigidbody2D>();
        GunPickUp script = tossedWeapon.GetComponent<GunPickUp>();

        script.isTossed = true;
        script.ammoOnWeapon = weaponsHoldingAmmo[activeWeaponNum];
        rb.AddForce(barrelPrefab.transform.right * 3000.0f);

        holdingWeapon = false;
        activeWeapon = null;
        weaponsHolding[activeWeaponNum] = null;
        weaponsHoldingScripts[activeWeaponNum] = null;
    }

    void WeaponAim()
    {
        if (!string.IsNullOrEmpty(weaponsHolding[activeWeaponNum]))
        {
            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (rotZ > 90 || rotZ < -90)
            {
                renderer.flipY = true;
                barrelPrefab.transform.localPosition = new Vector3(activeWeapon.barrelPosition.x, activeWeapon.barrelPosition.y * -1, activeWeapon.barrelPosition.z);
            }
            else
            {
                renderer.flipY = false;
                barrelPrefab.transform.localPosition = activeWeapon.barrelPosition;
            }
        }
    }
    public void WeaponPickUp(string weaponName, GunPickUp fromScript)
    {

        if (string.IsNullOrEmpty(weaponsHolding[0]))
        {
            weaponsHolding[0] = weaponName;
            weaponsHoldingScripts[0] = fromScript;
            activeWeaponNum = 0;
            weaponsHoldingAmmo[0] = weaponsHoldingScripts[0].ammoOnWeapon;
        }
        else if (string.IsNullOrEmpty(weaponsHolding[1]))
        {
            weaponsHolding[1] = weaponName;
            weaponsHoldingScripts[1] = fromScript;
            activeWeaponNum = 1;
            weaponsHoldingAmmo[1] = weaponsHoldingScripts[1].ammoOnWeapon;
        }
        else if (!string.IsNullOrEmpty(weaponsHolding[1]) && !string.IsNullOrEmpty(weaponsHolding[0]))
        {
            weaponsHolding[activeWeaponNum] = weaponName;
            weaponsHoldingScripts[activeWeaponNum] = fromScript;
            weaponsHoldingAmmo[activeWeaponNum] = weaponsHoldingScripts[activeWeaponNum].ammoOnWeapon;
        }
    }
    void ActiveWeaponUpdate()
    {
        if (string.IsNullOrEmpty(weaponsHolding[activeWeaponNum])) //check if player holding a weapon
            holdingWeapon = false;

        else if (!string.IsNullOrEmpty(weaponsHolding[activeWeaponNum]))
            holdingWeapon = true;

        if (holdingWeapon)
        {
            ammoText.text = weaponsHoldingAmmo[activeWeaponNum].ToString(); //change the ammo you see on the GUI
            activeWeapon = Array.Find(guns, gun => gun.gunName == weaponsHolding[activeWeaponNum]);
            barrelPrefab.transform.localPosition = activeWeapon.barrelPosition;
            GetComponent<SpriteRenderer>().sprite = activeWeapon.sprite;
        }
        else if (!holdingWeapon)
        {
            ammoText.text = null;
            weaponsHolding[activeWeaponNum] = null;
            weaponsHoldingScripts[activeWeaponNum] = null;
            activeWeapon = null;
            barrelPrefab.transform.localPosition = Vector3.zero;
            GetComponent<SpriteRenderer>().sprite = null;
            weaponsHoldingAmmo[activeWeaponNum] = 0;
        }

        if (!isReloading && reloadSlider.enabled)
            reloadSlider.value = 0;
    }

	#region Shooting Mechanics
	void IfThenShoot()
    {
        if (!activeWeapon.isAutomatic)
        {
            if (Input.GetButtonDown("Fire1") && weaponsHoldingAmmo[activeWeaponNum] > 0 && nextFire <= 0f)
            {
                Shoot();
                if (!isReloading && weaponsHoldingAmmo[activeWeaponNum] <= 0)
                    StartCoroutine("Reload");
            }
        }
        else if (activeWeapon.isAutomatic)
        {
            if (Input.GetButton("Fire1") && weaponsHoldingAmmo[activeWeaponNum] > 0 && nextFire <= 0f)
            {
                Shoot();
                if (!isReloading && weaponsHoldingAmmo[activeWeaponNum] <= 0)
                    StartCoroutine("Reload");
            }
        }
        if (Input.GetButtonDown("Fire1") && weaponsHoldingAmmo[activeWeaponNum] <= 0 && !isReloading)
        {
            Debug.Log("No Ammo");
            StartCoroutine("Reload");
        }
    }
    void Shoot()
    {
        GameObject mFlash = Instantiate(muzzleFlash, barrelPrefab.transform.position, transform.rotation);
        Instantiate(bulletPrefab, barrelPrefab.transform.position, transform.rotation);

        mFlash.GetComponent<MuzzleFlash>().firePoint = barrelPrefab;
        weaponsHoldingAmmo[activeWeaponNum]--;
        nextFire = activeWeapon.fireRate / 100;
        Debug.Log("Ammo: " + weaponsHoldingAmmo[activeWeaponNum]);
    }
    void IfThenReload()
    {
        if (holdingWeapon)
        {
            reloadSliderTime = Time.deltaTime / activeWeapon.reloadTime;
            if(!isReloading && Input.GetKeyDown(KeyCode.R) && weaponsHoldingAmmo[activeWeaponNum] != activeWeapon.maxAmmoCount)
                StartCoroutine("Reload");
        }
        else if (!holdingWeapon && isReloading)
        {
            Debug.Log("Canceled Reload");
            isReloading = false;
            StopCoroutine("Reload");
        }
    }
    IEnumerator Reload() //
    {
        reloadSlider.enabled = true;
        reloadSlider.value = 0;

        isReloading = true;
        Debug.Log("Started Reloading");
        while (reloadSlider.value != 1)
        {
            reloadSlider.value += reloadSliderTime;
            if (reloadSlider.value >= 1)
                reloadSlider.value = 1;
            yield return null;
        }
        if(reloadSlider.value == 1)
        {
            weaponsHoldingAmmo[activeWeaponNum] = activeWeapon.maxAmmoCount;
            Debug.Log("Reloaded");
            isReloading = false;
        }
    }
    #endregion Shooting Mechanics

    void WeaponChange()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            activeWeaponNum++;
            if (activeWeaponNum < 0)
            {
                activeWeaponNum = 1;
            }
            else if (activeWeaponNum > 1)
            {
                activeWeaponNum = 0;
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            activeWeaponNum--;
            if (activeWeaponNum < 0)
            {
                activeWeaponNum = 1;
            }
            else if (activeWeaponNum > 1)
            {
                activeWeaponNum = 0;
            }
        }
    }
	#endregion methods
}