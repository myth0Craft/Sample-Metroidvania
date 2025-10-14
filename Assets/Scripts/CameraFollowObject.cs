using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{

    [SerializeField] private Transform playerTransform;
    private PlayerMovement player;
    private float flipOffset = 2f;
    private float currentOffset;
    private float smoothSpeed = 3f;
    private float minMoveTime = 0.1f;
    private float moveTimer = 0f;
    private float lastActiveOffset = 0f;
    float lastDir = 0f;
    private bool facingRight = true;


    //private bool facingRight;
    //private bool checkGrounded = false;

    private void Awake()
    {
        player = playerTransform.GetComponent<PlayerMovement>();
        facingRight = player.getFacingDirection();
    }



    private void Update()
    {

        Vector3 targetPosition = playerTransform.position + new Vector3(getXOffset(), 0, 0);
        transform.position = targetPosition;


    }

    public float getXOffset()
    {


        float targetOffset = player.getFacingDirection() ? 0.7f : -0.7f;

        float xVel = player.getLinearVelocity().x;

        if (Mathf.Abs(xVel) > 0.2f)
        {
            moveTimer += Time.deltaTime;

            if (moveTimer > minMoveTime)
            {
                targetOffset = Mathf.Clamp(xVel, -0.7f, 0.7f);

            }
        }
        else
        {
            moveTimer = 0;
        }

        currentOffset = Mathf.Lerp(currentOffset, targetOffset, Time.deltaTime * smoothSpeed);

        return currentOffset;
    }


    public float getYOffset()
    {

        if (player.getLinearVelocity().y < 0)
        {
            return 0;
        } else
        {
            return 0;
        }
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
