using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{

    [SerializeField] private Transform playerTransform;
    private PlayerMovement player;
    private float flipOffset = 1f;
    private float smoothSpeed = 5f;

    private bool facingRight;
    //private bool checkGrounded = false;

    private void Awake()
    {
        player = playerTransform.GetComponent<PlayerMovement>();
        facingRight = player.getFacingDirection();
    }

    /*private void FixedUpdate()
    {
        if (checkGrounded)
        {
            if (player.IsGroundedBuffered())
            {
                checkGrounded = false;
                facingRight = !facingRight;
            }
        }
    }*/


    private void Update()
    {
        float targetOffset = facingRight ? flipOffset : -flipOffset;

        Vector3 targetPos = playerTransform.position;
        targetPos.x += targetOffset;
        targetPos.y = playerTransform.position.y;

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
    public void CallTurn()
    {
        /*if (player.IsGroundedBuffered())
        {
            facingRight = !facingRight;
        } else
        {
            checkGrounded = true;
        }*/
        facingRight = !facingRight;
        
    }
}
