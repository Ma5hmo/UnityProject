using System.Collections;
using UnityEngine;

public class ChestOpenAnim : MonoBehaviour
{
    public float vibrateSpeed;
    public float vibrateDistance;
    public float vibrateDuration;

    public Sprite chestClosed;
    public Sprite chestOpened;

    public GameObject lightUp;
    public GameObject halfCircleLight;
    public GameObject fullCircleLight;

    public Transform wpnHolder;
    SpriteRenderer rend;

    [HideInInspector]
    public bool isOpen = false;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();

        rend.sprite = chestClosed;
        lightUp.SetActive(false);
        halfCircleLight.SetActive(false);
        fullCircleLight.SetActive(true);
        wpnHolder.gameObject.SetActive(false);
        StartCoroutine(ChestVibrate());
    }

    public IEnumerator ChestVibrate()
    {
        float timer = 0;
        float sinFunc;
        while (timer < vibrateDuration * vibrateSpeed)
        {
            timer += Time.deltaTime * vibrateSpeed;
            sinFunc = Mathf.Sin(timer);

            transform.localPosition = new Vector3(sinFunc / vibrateDistance, 0, 0);
            yield return null;
        }
        transform.localPosition = Vector3.zero;
        rend.sprite = chestOpened;
        fullCircleLight.SetActive(false);
        lightUp.SetActive(true);
        halfCircleLight.SetActive(true);

        Gunpopup();
        isOpen = true;
    }
    Quaternion tarRot = Quaternion.Euler(0, 0, 20);
    Vector3 tarPos = new Vector3(0, 3.2f, -0.1f);
    Vector3 tarScale = new Vector3(2.5f, 2.5f, 1);
    public int speedPos;
    public int speedRot;
    public int speedScale;
    public void Gunpopup()
    {
        wpnHolder.gameObject.SetActive(true);

        wpnHolder.localScale = new Vector2(1.5f, 1.5f);
        wpnHolder.localPosition = new Vector3(0, 0, -0.1f);
        wpnHolder.rotation = Quaternion.Euler(Vector3.zero);

        StartCoroutine(GunpopRot());
        StartCoroutine(GunpopPos());
        StartCoroutine(GunpopScale());
    }
    public IEnumerator GunpopRot()
    {
        while (wpnHolder.rotation.eulerAngles.z < tarRot.eulerAngles.z)
        {
            wpnHolder.rotation = Quaternion.Euler(0, 0, wpnHolder.rotation.eulerAngles.z + Time.deltaTime * speedRot);
            yield return null;
        }
        wpnHolder.rotation = Quaternion.Euler(0, 0, 20);
    }
    public IEnumerator GunpopPos()
    {
        while (wpnHolder.localPosition.y < tarPos.y)
        {
            wpnHolder.localPosition += new Vector3(0, Time.deltaTime * speedPos, 0);
            yield return null;
        }
        wpnHolder.localPosition = tarPos;
    }
    public IEnumerator GunpopScale()
    {
        while (wpnHolder.localScale.x < tarScale.x && wpnHolder.localScale.y < tarScale.y)
        {
            wpnHolder.localScale += new Vector3(Time.deltaTime * speedScale, Time.deltaTime * speedScale, 0);
            yield return null;
        }
        wpnHolder.localScale = tarScale;
    }
}
