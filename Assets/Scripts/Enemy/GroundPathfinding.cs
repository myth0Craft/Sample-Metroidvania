using System.Runtime.CompilerServices;
using UnityEngine;

public class GroundPathfinding : MonoBehaviour
{
    private float PlayerXPos;
    private float currentXPos;

    private float currentDir = 1f;

    public float speed = 1f;

    public float accel = 1f;

    public Rigidbody2D body;
    
    private void Update()
    {
        PlayerXPos = PlayerMovement.instance.transform.position.x;

        currentDir = PlayerXPos < currentXPos ? -1 : 1;

        body.linearVelocity = new Vector2(Mathf.Lerp(body.linearVelocity.x, speed * currentDir, accel * Time.deltaTime), body.linearVelocity.y);
    }
}
