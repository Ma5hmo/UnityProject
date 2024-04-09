using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public bool rawMovement = false;

    public float health = 100f;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();        
    }
    void FixedUpdate()
    {
        if (rawMovement)
        {
            rb.position += new Vector2(Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed, Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed);
            //transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed, Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed, 0f));
        }
        else if (!rawMovement)
        {
            rb.position += new Vector2(Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed, Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed);
            //transform.Translate(new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed, Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed, 0f));
        }
    }
    private void LateUpdate()
    {
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void playerTakeDamage(float damage)
    {
        health -= damage;
    }
}
