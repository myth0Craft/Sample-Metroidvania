using UnityEngine;

public class SafeZoneTracker : MonoBehaviour
{
    public static SafeZoneTracker instance;

    private bool inSafeZone = true;

    private Vector2 lastSafeZone;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
        lastSafeZone = gameObject.transform.position;
    }

    public void UpdateLastSafeZone()
    {
        if (inSafeZone)
        {
            lastSafeZone = gameObject.transform.position;
        }
    }

    public void MoveParentToLastSafeZone()
    {
        transform.parent.gameObject.transform.position = lastSafeZone;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("UnsafeZone"))
        {
            inSafeZone = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("UnsafeZone"))
        {
            inSafeZone = true;
        }
    }
}
