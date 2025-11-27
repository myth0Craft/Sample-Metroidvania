using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;


public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private LayerMask groundLayer;
    private BoxCollider2D boxCollider;
    private Animator anim;

    [SerializeField] private Animator capeAnim;
    [SerializeField] private Animator armsAnim;
    [SerializeField] private Animator bodyAnim;
    [SerializeField] private Animator weaponsAnim;
    [SerializeField] private Animator legsAnim;
    [SerializeField] private Animator swordAnim;
    [SerializeField] private Animator shieldAnim;
    [SerializeField] private GameObject visual;

    private Vector3 sizeScale;
    


    //movement values
    private float speed = 4f;
    private float jumpStrength = 8f;

    private float baseGravity = 5f;
    private float lowJumpGravity = 7f;
    private float fallGravity = 10f;

    //ability usage/cooldown trackers
    private bool facingRight = true;
    private bool doubleJumpUsed = false;
    private bool dashUsed = false;
    

    //movement fine-tuning values
    private int maxJumpHoldFrames = 15;
    private float jumpIncreasePerFrameHeld = 0.5f;
    private int jumpHoldCounter = 0;

    private float jumpBufferTime = 0.1f;
    private float jumpBufferTimer = 0f;

    private float groundedRememberTime = 0.1f;
    private float groundedRememberTimer = 0f;
    private float wallRememberTime = 0.1f;
    private float wallRememberTimer = 0f;
    private float gravityMultiplier = 0.4f;
    private float accelGrounded = 40f;
    private float accelInAir = 25f;


    private float dashFrames = 0f;
    private float maxDashFrames = 15f;

    private float dashCooldown = 0;


    //TODO: FIX TIME
    private float dashCooldownTime = 30f;


    //inputs
    private PlayerControls controls;
    private float horizontalMovement;
    private float previousHorizontalMovement = 0;
    private bool jumpPressed;
    private bool dashPressed;
    private bool dashHeld;
    private bool jumpHeld;
    private bool wasJumpHeld;

    //camera
    private CameraFollowObject cameraFollowObject;
    private float fallSpeedYDampingChangeThreshold;


    void Awake()
    {
        //gets values from unity
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        controls = new PlayerControls();
        sizeScale = transform.localScale;
        groundLayer = LayerMask.GetMask("Ground");
        


        //maps controls
        controls.Player.Move.performed += ctx => horizontalMovement = ctx.ReadValue<Vector2>().x;
        controls.Player.Move.canceled += ctx => horizontalMovement = 0f;

        controls.Player.Jump.performed += ctx => jumpPressed = true;
        controls.Player.Dash.performed += ctx => dashPressed = true;
        controls.Player.Dash.started += ctx => dashHeld = true;
        controls.Player.Dash.canceled += ctx => dashHeld = false;


        controls.Player.Jump.canceled += ctx => jumpHeld = false;
        controls.Player.Jump.started += ctx => jumpHeld = true;
    }

    private void Start()
    {
        //fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampingChangeThreshold;
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
        if (IsGroundedBuffered() || StuckToWallBuffered())
        {
            doubleJumpUsed = false;
            dashUsed = false;
        }

        

        /*if (body.linearVelocity.y < fallSpeedYDampingChangeThreshold && CameraManager.instance.isLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        if (body.linearVelocity.y >= 0f && !CameraManager.instance.isLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }*/
        anim.SetBool("run", (horizontalMovement > 0.01f || horizontalMovement < -0.01f) && IsGroundedBuffered());
        //weaponsAnim.SetBool("moving", (horizontalMovement > 0.01f || horizontalMovement < -0.01f) && IsGroundedBuffered());
        capeAnim.SetBool("moving", (body.linearVelocity.x < -3f || body.linearVelocity.x > 3f) && !StuckToWallBuffered());
        bodyAnim.SetBool("moving", (horizontalMovement > 0.01f || horizontalMovement < -0.01f) && IsGroundedBuffered());
        //armsAnim.SetBool("moving", (horizontalMovement > 0.01f || horizontalMovement < -0.01f) && IsGroundedBuffered());
        legsAnim.SetBool("moving", (horizontalMovement > 0.01f || horizontalMovement < -0.01f) && IsGroundedBuffered());
        shieldAnim.SetBool("moving", (horizontalMovement > 0.01f || horizontalMovement < -0.01f) && IsGroundedBuffered());
        swordAnim.SetBool("moving", (horizontalMovement > 0.01f || horizontalMovement < -0.01f) && IsGroundedBuffered());


    }


    private void FixedUpdate()
    {

        if (dashFrames > 0)
        {
            dashFrames--;
        }

        if (dashCooldown > 0)
        {
            dashCooldown--;
        }

        //base left/right movement
        MoveHorizontal();

        previousHorizontalMovement = horizontalMovement;


        if (StuckToWallBuffered())
        {
            dashHeld = false;
            dashPressed = false;
            dashFrames = 0;
            dashCooldown = 15;
        }
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
            if (StuckToWallBuffered() && !IsGroundedBuffered()) { 
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
        if (dashPressed && !dashUsed)
        {
            if (dashCooldown <= 0)
            {
                dashCooldown = dashCooldownTime;
                dashFrames = maxDashFrames;
                dashPressed = false;
            }
            if (IsGroundedBuffered())
            {
                dashPressed = false;
            }
        }

        

        //apply current gravity
        body.gravityScale = getGravity() * gravityMultiplier;
        if (StuckToWallBuffered())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, Mathf.Max(body.linearVelocity.y, -5f));

        } else
            body.linearVelocity = new Vector2(body.linearVelocity.x, Mathf.Max(body.linearVelocity.y, -15f));

    }

    public void enableSword()
    {
        swordAnim.SetBool("hasSword", true);
    }

    public void disableSword()
    {
        swordAnim.SetBool("hasSword", false);
    }

    public void enableShield()
    {
        shieldAnim.SetBool("hasShield", true);
    }

    public void disableShield()
    {
        shieldAnim.SetBool("hasShield", false);
    }

    public void bodyDrawSword()
    {
        bodyAnim.SetTrigger("drawSword");
    }



    //moves the player left or right
    private void MoveHorizontal()
    {
        /*float targetSpeed = input * speed;
        body.linearVelocity = new Vector2(targetSpeed, body.linearVelocity.y);*/

        if (dashFrames > 0 && !StuckToWallBuffered())
        {
           
            dashUsed = true;
            float xVel = getFacingDirection() ? 10 : -10;
            body.linearVelocity = new Vector2(xVel, 0);
        }
        else
        {

            float xMultiplier = dashHeld ? 1.70f : 1;
            /* if (!dashUsed && dashFrames > 0)
             {
                 multiplier = 20;
             print("dashed");
             }*/
            float accel = IsGroundedBuffered() ? accelGrounded : accelInAir;
            float newVelX = Mathf.MoveTowards(body.linearVelocity.x, horizontalMovement * speed * xMultiplier, accel * Time.fixedDeltaTime);
            body.linearVelocity = new Vector2(newVelX, body.linearVelocity.y);
            if (Mathf.Abs(horizontalMovement) > 0.01f)
                TurnSprite();
        }
        
        
    }

    //applies sideways motion for dashing
    private void Dash()
    {

        
    }

    public Vector3 getLinearVelocity()
    {
        return body.linearVelocity;
    }

    public bool getFacingDirection()
    {
        return facingRight;
    }

    private void TurnSprite()
    {
        //Vector3 scale = transform.localScale;
        //scale.x = horizontalMovement > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        //scale.y = horizontalMovement > 0 ? 0f : 180f;
        //transform.localScale = scale;

        /*if (horizontalMovement > 0)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            facingRight = true;
        }else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            facingRight = false;
        }*/

        bool shouldFaceRight = horizontalMovement > 0;
        if (shouldFaceRight != facingRight)
        {
            Vector3 rot = visual.transform.rotation.eulerAngles;
            rot.y = shouldFaceRight ? 0f : 180f;
            visual.transform.rotation = Quaternion.Euler(rot);
            //body.transform.localScale = new Vector3(shouldFaceRight ? 1 : -1, 1, 1);

            facingRight = shouldFaceRight;
        }
        

    }

    //returns the current gravity
    private float getGravity()
    {
        float finalGravity;
        if (body.linearVelocity.y > 0 && !jumpHeld)
            finalGravity = lowJumpGravity;
        else if (body.linearVelocity.y < 0)
            finalGravity = fallGravity;
        else
            finalGravity = baseGravity;

        if (StuckToWallBuffered() && body.linearVelocityY <= 0f)
        {
            finalGravity *= 0.1f;
        }

        if (dashHeld && body.linearVelocity.y < 0)
        {
            finalGravity *= 0.6f;
        }

        if (dashFrames > 0f && !StuckToWallBuffered())
        {
            finalGravity = 0f;
        }

        return finalGravity;


    }

    //checks if player is on ground
    public bool IsGrounded()
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
    public bool IsGroundedBuffered()
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
        Vector2 direction = getFacingDirection() ? Vector2.right : Vector2.left;

        bool isTouchingWall = Physics2D.Raycast(boxCollider.bounds.center, direction, boxCollider.bounds.size.x - 0.05f, groundLayer);

        /*RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            direction,
            0.1f,
            groundLayer
        );*/

        //return hit.collider != null
        return isTouchingWall && body.linearVelocityY <= 0.1;
    }

    private bool StuckToWallBuffered()
    {
        if (StuckToWall())
            wallRememberTimer = wallRememberTime;
        else
            wallRememberTimer -= Time.deltaTime;
        return wallRememberTimer > 0f;
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


    

    //makes the player jump off of a wall
    private void ExecuteWallJump()
    {
        /*float accel = IsGroundedBuffered() ? accelGrounded : accelInAir;
        float newVelX = Mathf.MoveTowards(body.linearVelocity.x, -horizontalMovement * speed, accel * Time.fixedDeltaTime);*/


        float wallJumpHorizontalForce = facingRight ? -5.5f : 5.5f;
        body.linearVelocity = new Vector2(wallJumpHorizontalForce, jumpStrength * 1.3f);
        //body.linearVelocity = new Vector2(body.linearVelocityX, jumpStrength * 1.2f);

    }
}