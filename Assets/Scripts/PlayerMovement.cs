using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;


public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private LayerMask groundLayer;
    private BoxCollider2D boxCollider;
    private Vector3 sizeScale;
    


    //movement values
    private float speed = 4f;
    private float jumpStrength = 8f;

    private float baseGravity = 5f;
    private float lowJumpGravity = 7f;
    private float fallGravity = 10f;

    //ability usage/cooldown trackers
    private bool doubleJumpUsed = false;
    private bool dashUsed = false;
    private int dashCooldown = 0;

    //movement fine-tuning values
    private int maxJumpHoldFrames = 15;
    private float jumpIncreasePerFrameHeld = 0.5f;
    private int jumpHoldCounter = 0;

    private float jumpBufferTime = 0.1f;
    private float jumpBufferTimer = 0f;

    private float groundedRememberTime = 0.1f;
    private float groundedRememberTimer = 0f;
    private float gravityMultiplier = 0.4f;
    private float accelGrounded = 40f;
    private float accelInAir = 30f;


    //inputs
    private PlayerControls controls;
    private float horizontalMovement;
    private float previousHorizontalMovement = 0;
    private bool jumpPressed;
    private bool dashPressed;
    private bool jumpHeld;
    private bool wasJumpHeld;



    void Awake()
    {
        //gets values from unity
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        controls = new PlayerControls();
        sizeScale = transform.localScale;
        groundLayer = LayerMask.GetMask("Ground");
        

        //maps controls
        controls.Player.Move.performed += ctx => horizontalMovement = ctx.ReadValue<Vector2>().x;
        controls.Player.Move.canceled += ctx => horizontalMovement = 0f;

        controls.Player.Jump.performed += ctx => jumpPressed = true;
        controls.Player.Dash.performed += ctx => dashPressed = true;

        controls.Player.Jump.canceled += ctx => jumpHeld = false;
        controls.Player.Jump.started += ctx => jumpHeld = true;
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    
    private void Update()
    {
        //resets double jump if player is on ground
        if (IsGroundedBuffered())
        {
            doubleJumpUsed = false;
            dashUsed = false;
        }
    }

    private void FixedUpdate()
    {
        //Time.timeScale = 0.8f;
        //base left/right movement
        MoveHorizontal();

        previousHorizontalMovement = horizontalMovement;



        //jumping

        if (jumpPressed)
        {
            jumpBufferTimer = jumpBufferTime;
            jumpPressed = false;
        }
        else
        {
            jumpBufferTimer -= Time.fixedDeltaTime;
        }


        if (jumpBufferTimer > 0f)
        {
            if (StuckToWall()) { 
                ExecuteWallJump();
                jumpBufferTimer = 0f;
            }
            else
            {
                Jump();
                jumpBufferTimer = 0f;
            }
        }

        ApplyJumpHold();

        if (wasJumpHeld && !jumpHeld)
        {
            OnJumpReleased();
        }

        wasJumpHeld = jumpHeld;

        //dashing
        /*if (dashPressed)
        {
            Dash();
            dashPressed = false;
        }
        if (dashCooldown > 0)
        {
            dashCooldown--;
            
        }*/

        //apply current gravity
        body.gravityScale = getGravity() * gravityMultiplier;
        
    }


    //moves the player left or right
    private void MoveHorizontal()
    {
        /*float targetSpeed = input * speed;
        body.linearVelocity = new Vector2(targetSpeed, body.linearVelocity.y);*/

        float accel = IsGroundedBuffered() ? accelGrounded : accelInAir;
        float newVelX = Mathf.MoveTowards(body.linearVelocity.x, horizontalMovement * speed, accel * Time.fixedDeltaTime);
        body.linearVelocity = new Vector2(newVelX, body.linearVelocity.y);
        if (Mathf.Abs(horizontalMovement) > 0.01f)
            TurnSprite(horizontalMovement > 0);
    }

    private void TurnSprite(bool facingRight)
    {
        Vector3 scale = transform.localScale;
        scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        //scale.y = facingRight ? 0f : 180f;
        transform.localScale = scale;

    }

    //returns the current gravity
    private float getGravity()
    {
        if (body.linearVelocity.y > 0 && !jumpHeld)
            return lowJumpGravity;
        if (body.linearVelocity.y < 0)
            return fallGravity;
        return baseGravity;
    }

    //checks if player is on ground
    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
        boxCollider.bounds.center,
        boxCollider.bounds.size,
        0f,
        Vector2.down,
        0.1f,
        groundLayer
        );

        return hit.collider != null;
    }

    //checks if player was on ground in last 0.1s
    private bool IsGroundedBuffered()
    {
        if (IsGrounded())
            groundedRememberTimer = groundedRememberTime;
        else
            groundedRememberTimer -= Time.deltaTime;

        return groundedRememberTimer > 0f;
    }

    //checks if player is attached to a wall
    private bool StuckToWall()
    {
        return false;
    }

    //applies upwards jump motion for jumps and double jumps
    private void Jump()
    {
        if (IsGroundedBuffered())
        {
            //jump logic
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpStrength);
            //body.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
            jumpHoldCounter = maxJumpHoldFrames;
            
        } else if (!doubleJumpUsed)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpStrength);
            //body.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
            jumpHoldCounter = maxJumpHoldFrames;
            doubleJumpUsed = true;
        }
    }

    private void ApplyJumpHold()
    {
        if (jumpHeld && jumpHoldCounter > 0)
        {
            body.linearVelocity += Vector2.up * (jumpStrength * jumpIncreasePerFrameHeld) * Time.fixedDeltaTime;
            //body.linearVelocity = new Vector2(body.linearVelocity.x, jumpStrength);
            jumpHoldCounter--;
        }
    }

    private void OnJumpReleased()
    {
        if (body.linearVelocity.y > 0)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y * 0.4f);
        }
    }


    //applies sideways motion for dashing
    private void Dash()
    {
        
        if (dashCooldown <= 0 && !dashUsed)
        {
            dashCooldown = 200;
            body.linearVelocity = new Vector2(horizontalMovement * speed * 30, 0);
            dashUsed = true;
        }
    }

    //makes the player jump off of a wall
    private void ExecuteWallJump()
    {

    }
}
