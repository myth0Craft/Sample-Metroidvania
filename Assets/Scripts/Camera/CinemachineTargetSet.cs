using Unity.Cinemachine;
using UnityEngine;

public class CinemachineTargetSet : MonoBehaviour
{

    private CinemachineCamera cam;
    private GameObject trackingTarget;
    void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        trackingTarget = GameObject.FindGameObjectWithTag("CameraFollow");
        
    }
}
