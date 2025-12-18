using UnityEngine;

public class CameraSwitchTrigger : MonoBehaviour
{
    private GameObject cinemachineCam;
    private CameraManager manager;

    [SerializeField] private BoxCollider2D leftCollider;
    [SerializeField] private BoxCollider2D rightCollider;


    void Awake()
    {
        cinemachineCam = GameObject.FindGameObjectWithTag("CinemachineCamera");
        manager = cinemachineCam.GetComponent<CameraManager>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - collision.bounds.center).normalized;
            //manager.SwapCamera(leftCollider, rightCollider, exitDirection);
        }
    }
}
