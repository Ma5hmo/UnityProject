using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    // fix the active weapon update callouts

    #region Variables
    [HideInInspector]
    public Weapon activeWeapon { get { return hotbarWpns[hotbarNum]; } }

    [HideInInspector]
    public bool isInventoryFull { get {
            if (hotbarWpns[0] != null && hotbarWpns[1] != null)
                return true;
            else
                return false; } }
    Weapon[] hotbarWpns = new Weapon[2];
    int[] wpnsAmmo = new int[2];

    int hotbarNum = 0;

    public float bulletSpeed;
    public float weaponTossForce;

    public GameObject bulletPrefab;
    public GameObject muzzleFlash;
    GameObject barrel;

    bool isReloading;

    [Header("GUI References")]
    public Text ammoText;
    public Slider reloadSlider;
    public Image[] hotbarSlots = new Image[2];

    public Sprite emptyWeaponSlot;
    public Sprite emptySelectedWeaponSlot;

    float nextFire = 0f;
    #endregion Variables

    #region Main Functions

    void Awake()
    {
        barrel = GameObject.FindGameObjectWithTag("FirePoint");
    }
    void Start()
    {
        ActiveWeaponUpdate();
    }

    void Update()
    {
        Debug.Log(isInventoryFull);
        WeaponChange();
        if (activeWeapon != null)
        {
            WeaponAim();
            IfThenShoot();
            if (!isReloading && Input.GetKeyDown(KeyCode.R) && wpnsAmmo[hotbarNum] != activeWeapon.maxAmmoCount)
            {
                StartCoroutine(Reload(hotbarNum));
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                ThrowWeapon();
            }
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
        ammoText.text = wpnsAmmo[hotbarNum].ToString(); //change the ammo you see on the GUI
        if (!isReloading && reloadSlider.enabled) //hide the reload slider if not reloading
        {
            reloadSlider.value = 0;
        }
    }
    #endregion MainFunctions

    #region Methods

    void ThrowWeapon()
    {
        GameObject tossedWeapon = Instantiate(activeWeapon.objectToThrow, transform.position, Quaternion.identity);

        Rigidbody2D rb = tossedWeapon.GetComponent<Rigidbody2D>();
        GunPickUp script = tossedWeapon.GetComponent<GunPickUp>();

        script.isTossed = true;
        script.ammoOnWeapon = wpnsAmmo[hotbarNum];
        rb.AddForce(barrel.transform.up * 3000.0f);

        hotbarWpns[hotbarNum] = null;
        ActiveWeaponUpdate();

        if (isReloading)
        {
            reloadSlider.value = 0;
            isReloading = false;
            reloadSlider.enabled = false;
        }
    }

    void WeaponAim()
    {
        if (hotbarWpns[hotbarNum])
        {
            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (rotZ > 90 || rotZ < -90)
            {
                renderer.flipY = true;
                barrel.transform.localPosition = new Vector2(activeWeapon.barrelPosition.x, activeWeapon.barrelPosition.y * -1);
            }
            else
            {
                renderer.flipY = false;
                barrel.transform.localPosition = activeWeapon.barrelPosition;
            }
        }
    }
    public void WeaponPickUp(Weapon wpn, int ammo)
    {
        if (hotbarWpns[0] == null)
        {
            hotbarWpns[0] = wpn;
            hotbarNum = 0;
            wpnsAmmo[0] = ammo;
        }
        else if (hotbarWpns[1] == null)
        {
            hotbarWpns[1] = wpn;
            hotbarNum = 1;
            wpnsAmmo[1] = ammo;
        }
        else
        {
            ThrowWeapon();
            hotbarWpns[hotbarNum] = wpn;
            wpnsAmmo[hotbarNum] = ammo;
        }
        ActiveWeaponUpdate();
    }
    void ActiveWeaponUpdate()
    {
        if (activeWeapon != null)
        {
            barrel.GetComponent<Light2D>().enabled = true;
            GetComponent<SpriteRenderer>().sprite = activeWeapon.sprite;
        }
        else
        {
            ammoText.text = "";
            barrel.transform.localPosition = Vector3.zero;
            barrel.GetComponent<Light2D>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = null;
            wpnsAmmo[hotbarNum] = 0;
        }

        for (int i = 0; i < hotbarSlots.Length; i++) //update hotbar GUI
        {
            if (hotbarNum == i && hotbarWpns[i] != null)
            {
                hotbarSlots[i].sprite = hotbarWpns[i].WeaponSlotSpriteSelected;
            }
            else if (hotbarWpns[i] != null)
            {
                hotbarSlots[i].sprite = hotbarWpns[i].WeaponSlotSprite;
            }
            else if (hotbarNum == i)
            {
                hotbarSlots[i].sprite = emptySelectedWeaponSlot;
            }
            else
            {
                hotbarSlots[i].sprite = emptyWeaponSlot;
            }
        }
    }

    #region Shooting Mechanics
    void IfThenShoot()
    {
        if (!activeWeapon.isAutomatic)
        {
            if (Input.GetButtonDown("Fire1") && wpnsAmmo[hotbarNum] > 0 && nextFire <= 0f && !isReloading)
            {
                Shoot();
                if (wpnsAmmo[hotbarNum] <= 0)
                    StartCoroutine(Reload(hotbarNum));
            }
        }
        else if (activeWeapon.isAutomatic)
        {
            if (Input.GetButton("Fire1") && wpnsAmmo[hotbarNum] > 0 && nextFire <= 0f && !isReloading)
            {
                Shoot();
                if (wpnsAmmo[hotbarNum] <= 0)
                    StartCoroutine(Reload(hotbarNum));
            }
        }
        if (Input.GetButtonDown("Fire1") && wpnsAmmo[hotbarNum] <= 0 && !isReloading)
        {
            StartCoroutine(Reload(hotbarNum));
        }
    }
    void Shoot()
    {
        GameObject mFlash = Instantiate(muzzleFlash, barrel.transform.position, transform.rotation);
        Instantiate(bulletPrefab, barrel.transform.position, transform.rotation);

        mFlash.GetComponent<MuzzleFlash>().firePoint = barrel;
        wpnsAmmo[hotbarNum]--;
        nextFire = activeWeapon.fireRate / 100;
    }
    IEnumerator Reload(int WeaponNumber)
    {
        reloadSlider.enabled = true;
        reloadSlider.value = 0;
        isReloading = true;

        while (reloadSlider.value != 1)
        {
            if (hotbarNum != WeaponNumber)
            {
                isReloading = false;
                yield break;
            }
            else if (hotbarNum == WeaponNumber)
            {
                float reloadSliderTime = Time.deltaTime / activeWeapon.reloadTime;
                reloadSlider.value += reloadSliderTime;
                if (reloadSlider.value >= 1)
                    reloadSlider.value = 1;
                yield return null;
            }
        }
        if (reloadSlider.value == 1)
        {
            wpnsAmmo[hotbarNum] = activeWeapon.maxAmmoCount;
            isReloading = false;
        }
    }
    #endregion Shooting Mechanics

    void WeaponChange()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            hotbarNum++;
            if (hotbarNum < 0)
            {
                hotbarNum = 1;
            }
            else if (hotbarNum > 1)
            {
                hotbarNum = 0;
            }
            ActiveWeaponUpdate();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            hotbarNum--;
            if (hotbarNum < 0)
            {
                hotbarNum = 1;
            }
            else if (hotbarNum > 1)
            {
                hotbarNum = 0;
            }
            ActiveWeaponUpdate();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hotbarNum = 0;
            ActiveWeaponUpdate();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hotbarNum = 1;
            ActiveWeaponUpdate();
        }
    }
    #endregion methods
}