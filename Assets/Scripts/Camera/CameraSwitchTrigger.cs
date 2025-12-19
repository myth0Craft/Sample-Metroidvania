using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitchTrigger : MonoBehaviour
{

    [SerializeField] private CinemachineCamera leftCam;
    [SerializeField] private CinemachineCamera rightCam;
    private BoxCollider2D collider;

    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - collider.bounds.center).normalized;
            if (leftCam != null && rightCam != null)
            {
                if (exitDirection.x > 0f)
                {
                    leftCam.Priority = 0;
                    rightCam.Priority = 10;


                }
                else if (exitDirection.x < 0f)
                {
                    leftCam.Priority = 10;
                    rightCam.Priority = 0;

                }
            } else
            {
                if (exitDirection.x > 0f && leftCam != null)
                    leftCam.Priority = 0;
                else if (exitDirection.x > 0f && rightCam != null)
                    rightCam.Priority = 10;
                else if (exitDirection.x < 0f && leftCam != null)
                    leftCam.Priority = 10;
                else if (exitDirection.x < 0f && rightCam != null)
                    rightCam.Priority = 0;
            }
        }
    }
}
