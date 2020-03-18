using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public bool rawMovement = false;

    public float health = 100f;
    void Update()
    {
        if (rawMovement)
        {
            transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed, Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed, 0f));
        }else if (!rawMovement)
        {
            transform.Translate(new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed, Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed, 0f));

        }
    }
    public void playerTakeDamage(float damage)
    {
        health -= damage;
    }
}
