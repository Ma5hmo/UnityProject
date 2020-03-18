using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float health = 100f;
    public float speed = 2f;
    public float damage = 10;

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject); //die
        }
    }

    public void enemyTakeDamage(float damage)
    {
        health -= damage;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerMovement playerScript = col.GetComponent<PlayerMovement>();

            playerScript.playerTakeDamage(damage);
        }
    }
}
