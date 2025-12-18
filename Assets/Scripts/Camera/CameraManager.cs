using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //private GameObject[] allCinemachineGOs;
    //private CinemachineCamera[] allCinemachineCams;


    private void Awake()
    {
        /*allCinemachineGOs = GameObject.FindGameObjectsWithTag("CinemachineCamera");
        for (int i = 0; i < allCinemachineGOs.Length; i++) 
        { 
            allCinemachineCams[i] = allCinemachineGOs[i].GetComponent<CinemachineCamera>();
            print(allCinemachineCams[i]);
        }*/
        

    }
    //[SerializeField] private CinemachineCamera CamLeft;
    //[SerializeField] private CinemachineCamera CamRight;

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        Vector2 exitDirection = (collision.transform.position - collision.bounds.center).normalized;
    //        SwapCamera(CamLeft, CamRight, exitDirection);
    //    }
    //}

    //public void SwapCamera(BoxCollider2D cameraFromLeft, BoxCollider2D cameraFromRight, Vector2 triggerExitDirection)
    //{
    //    if (triggerExitDirection.x > 0f)
    //    {
    //        confiner.InvalidateBoundingShapeCache();
    //        confiner.BoundingShape2D = cameraFromRight;


    //    }
    //    else if (triggerExitDirection.x < 0f)
    //    {
    //        confiner.InvalidateBoundingShapeCache();
    //        confiner.BoundingShape2D = cameraFromLeft;

    //    }
    //}
}
