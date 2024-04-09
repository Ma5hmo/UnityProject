using UnityEngine;

public class camFix : MonoBehaviour
{
    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
    }
}
