using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public bool playerInRange { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
