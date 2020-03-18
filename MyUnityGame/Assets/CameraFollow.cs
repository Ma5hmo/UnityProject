using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime;
    public Vector2 offset;
    private Vector3 speed;

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + new Vector3(offset.x, offset.y, -1);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref speed, smoothTime);

    }
}
