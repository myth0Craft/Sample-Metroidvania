using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;


public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D body;

    private bool doubleJumpUsed = false;
    private int dashCooldown = 0;

    [SerializeField] private LayerMask groundLayer;
    private BoxCollider2D boxCollider;

    //movement values
    public float speed = 5f;
    public float jumpStrength = 10f;
   

    public int baseGravity = 10;
    public int lowJumpGravity = 16;
    public int fallGravity = 20;

    //inputs

    private PlayerControls controls;
    private float horizontalMovement;
    private bool jumpPressed;
    private bool dashPressed;
    private bool jumpHeld;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        controls = new PlayerControls();
        

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
        if (IsGrounded())
        {
            doubleJumpUsed = false;
        }
    }

    private void FixedUpdate()
    {
        MoveHorizontal(horizontalMovement);
        if (jumpPressed)
        {
            if (StuckToWall())
            {
                ExecuteWallJump();
            }
            else
            {
                Jump();
            }
            jumpPressed = false;
        }

        if (dashPressed)
        {
            Dash();
            dashPressed = false;
        }
        if (dashCooldown > 0)
        {
            dashCooldown--;
        }

        body.gravityScale = getGravity();
        
    }



    private void MoveHorizontal(float input)
    {
        body.linearVelocity = new Vector2(input * speed, body.linearVelocity.y);
    }

    private float getGravity()
    {
        if (body.linearVelocity.y > 0 && !jumpHeld)
            return lowJumpGravity;
        if (body.linearVelocity.y < 0)
            return fallGravity;
        return baseGravity;
    }

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

    private bool StuckToWall()
    {
        return false;
    }

    private void Jump()
    {
        
        if (IsGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpStrength);
        }
        else if (!doubleJumpUsed)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpStrength);
            doubleJumpUsed = true;
        }
    }



    private void Dash()
    {
        print("dashed");
        if (dashCooldown <= 0)
        {
            dashCooldown = 200;
            body.linearVelocity = new Vector2(horizontalMovement * speed * 30, 0);
        }
    }

    private void ExecuteWallJump()
    {

    }
}
