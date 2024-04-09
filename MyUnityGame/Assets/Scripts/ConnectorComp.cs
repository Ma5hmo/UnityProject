using UnityEngine;

public class ConnectorComp : MonoBehaviour
{
    public GameObject player;
    
    public bool playerTouching;

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
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
    }
    /*/void OnBecameInvisible()
    {
        Destroy(gameObject);
    }/*/
}
