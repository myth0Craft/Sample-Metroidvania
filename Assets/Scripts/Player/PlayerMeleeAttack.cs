using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    private PlayerControls controls;
    private PlayerMovement playerMovement;
    private bool attackPressed;
    private float attackDurationSeconds = 0.2f;
    private float attackTimer = 0;
    private float attackCooldownDurationSeconds = 0.2f;
    private float attackCooldownTimer = 0;
    //BoxCollider2D playerBox;
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private Animator attackAnimator;
    private BoxCollider2D attackCollider;
    private bool oldFacingRight;

    private void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        //playerBox = GetComponentInParent<BoxCollider2D>();
        attackCollider = attackHitbox.GetComponent<BoxCollider2D>();
        controls = new PlayerControls();
        controls.Player.Attack.performed += ctx => attackPressed = true;
        oldFacingRight = playerMovement.getFacingDirection();
        attackHitbox.SetActive(false);
    }

    private void Update()
    {
        //set the attack hitbox direction
        Vector3 playerPos = playerMovement.transform.position;
        Vector3 offsetVector = playerMovement.getFacingDirection() ? new Vector3(0.5f, 0.25f, 0) : new Vector3(-0.5f, 0.25f, 0);
        attackHitbox.transform.position = playerPos += offsetVector;


        //handle in-between attack cooldown
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer < 0)
                attackCooldownTimer = 0;
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer < 0)
                attackTimer = 0;

            if (attackTimer == 0)
            {
                attackHitbox.SetActive(false);
                attackCooldownTimer = attackCooldownDurationSeconds;
            }
        }


        if (attackPressed)
        {

            if ((attackCooldownTimer == 0 || attackCooldownTimer < 0.03) && attackTimer == 0) { 
                StartAttack();
                
            }
            attackPressed = false;
        }
        

        

    }



    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    public void StartAttack()
    {
        attackHitbox.SetActive(true);
        attackTimer = attackDurationSeconds;
        attackAnimator.SetTrigger("SwingSword");
        
    }
}
