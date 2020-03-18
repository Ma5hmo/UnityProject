using UnityEngine;
using System.Collections;

public class MuzzleFlash : MonoBehaviour
{
    [HideInInspector]
    public GameObject firePoint;

    public float fps = 30.0f;
    public Sprite[] frames;

    private SpriteRenderer rendererMy;

    void Start()
    {
        rendererMy = GetComponent<SpriteRenderer>();
        StartCoroutine(Animate());
    }
    void Update()
    {
        transform.position = firePoint.transform.position;
    }

    IEnumerator Animate()
    {
        foreach (Sprite s in frames)
        {
            rendererMy.sprite = s;
            yield return new WaitForSeconds(1 / fps);
        }
        Destroy(gameObject);
    }
}