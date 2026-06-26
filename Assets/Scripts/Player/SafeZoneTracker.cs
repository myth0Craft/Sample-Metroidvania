
using Unity.Cinemachine;
using UnityEngine;

public class SafeZoneTracker : MonoBehaviour
{
    public static SafeZoneTracker instance;



    private Vector2 respawnCheckpoint;

   /* public CinemachineCamera camToLoad;
    public CinemachineCamera camToUnload;*/

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
        respawnCheckpoint = transform.parent.gameObject.transform.position;
    }


    

    public void MoveParentToLastSafeZone()
    {
        transform.parent.gameObject.transform.position = respawnCheckpoint;

        /*if (camToLoad != null)
        {
            camToLoad.Priority = 10;
        }

        if (camToUnload != null)
        {
            camToUnload.Priority = 0;
        }*/

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("RespawnCheckpoint"))
        {
            respawnCheckpoint = transform.parent.gameObject.transform.position;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("RespawnCheckpoint"))
        {
            respawnCheckpoint = transform.parent.gameObject.transform.position;
        }
    }
}
