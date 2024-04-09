using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoomComp : MonoBehaviour
{
	public List<GameObject> enemies = new List<GameObject>();

	public bool playerTouching;

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Enemy"))
		{
			enemies.Add(col.gameObject);
		}
	}
	void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			playerTouching = true;
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			playerTouching = false;
		}
		if (col.CompareTag("Enemy"))
		{
			enemies.Remove(col.gameObject);
		}
	}

	/*/void OnBecameInvisible()
	{
		Destroy(gameObject);
	}/*/
}
