using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class CinemachineTargetSet : MonoBehaviour
{

    //private CinemachineCamera cam;
    //private GameObject trackingTarget;
    //void Start()
    //{
    //    cam = GetComponent<CinemachineCamera>();
    //    trackingTarget = GameObject.FindGameObjectWithTag("CameraFollow");

    //}

    private void Start()
    {
        var vcam = GetComponent<CinemachineCamera>();
        var target = GameObject.FindGameObjectWithTag("CameraFollow");
        if (target != null)
        {
            vcam.Follow = target.transform;
        }
    }

    private void OnEnable()
    {
        var vcam = GetComponent<CinemachineCamera>();
        var target = GameObject.FindGameObjectWithTag("CameraFollow");
        if (target != null)
        {
            vcam.Follow = target.transform;
        }
    }
}
