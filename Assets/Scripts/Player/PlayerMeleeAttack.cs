using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public enum CombatState
{
    Idle,
    Startup,
    Active,
    Cooldown,
    Blocking
}

public class PlayerMeleeAttack : MonoBehaviour
{

    public CombatState currentCombatState { get; private set; }


    public static PlayerMeleeAttack instance;
    private PlayerControls controls;
    private PlayerMovement playerMovement;
    private float attackHitboxActiveDurationSeconds = 0.12f;
    public bool attackHitboxActive { get; private set; } = false;

    private float attackCooldownDurationSeconds = 1f;
    [SerializeField] private GameObject attackHitbox;

    [SerializeField] private bool attackDebug;
    public bool attackDebugActive { get; private set; } = false;
    private BoxCollider2D attackCollider;

    public int comboNum = 0;

    private float attackStartupSeconds = 0.28f;

    private bool attackQueued = false;

    private Coroutine currentCombatCoroutine;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }


        playerMovement = GetComponentInParent<PlayerMovement>();
        attackCollider = attackHitbox.GetComponent<BoxCollider2D>();
        controls = PlayerData.getControls();
        controls.Player.Attack.performed += OnAttackPressed;
        attackHitbox.SetActive(false);
        attackDebugActive = attackDebug;

        currentCombatState = CombatState.Idle;
    }





    private void OnDestroy()
    {
        controls.Player.Attack.performed -= OnAttackPressed;
    }

    void OnEnable()
    {
        if (controls != null)
        {
            controls.Player.Enable();
        }
        //controls.Player.Attack.performed += OnAttackPressed;
    }

    void OnDisable()
    {
        if (controls != null)
        {
            controls.Player.Disable();
        }
        controls.Player.Attack.performed -= OnAttackPressed;
    }

    private void OnAttackPressed(InputAction.CallbackContext context)
    {
        if ((PlayerData.swordUnlocked || attackDebugActive) && !PlayerData.gamePaused)
        {
            if (comboNum >= 2)
            {
                return;
            }


            Debug.Log("Clicked with combat state " + currentCombatState + " at combo stage " + comboNum);
            if (playerMovement.currentHorizontalState == HorizontalState.Dashing && playerMovement.IsGroundedBuffered()
                )
            {
                PerformDashAttack();
                return;
            }


            if (currentCombatState == CombatState.Idle)
            {
                //attackQueued = true;
                currentCombatState = CombatState.Startup;
                comboNum = 0;
                StartAttack();
                return;
            }

            if (currentCombatState == CombatState.Startup || currentCombatState == CombatState.Cooldown || currentCombatState == CombatState.Active)
            {
                if (!attackQueued)
                {
                    comboNum++;
                }
                attackQueued = true;
                return;
            }
        }
    }

    private void PerformDashAttack()
    {
        PlayerAnimationManager.instance.LungeAttack();
        PlayerAnimationManager.instance.disableSword();
        PlayerMovement.instance.currentHorizontalState = HorizontalState.Dashing;
        //PlayerMovement.instance.OnSprintCanceled();
        PlayerMovement.instance.OnDashAttack();
    }


    private IEnumerator AttackCoroutine()
    {
        comboNum = 0;
        do
        {
            attackQueued = false;

            currentCombatState = CombatState.Startup;
            yield return new WaitForSeconds(attackStartupSeconds);

            UpdateFacingDirection();

            currentCombatState = CombatState.Active;
            attackHitbox.SetActive(true);
            attackHitboxActive = true;
            yield return new WaitForSeconds(attackHitboxActiveDurationSeconds);
            attackHitbox.SetActive(false);
            attackHitboxActive = false;

            currentCombatState = CombatState.Cooldown;



            float cooldownTimer = 0f;
            bool comboContinued = false;

            while (cooldownTimer < attackCooldownDurationSeconds)
            {
                cooldownTimer += Time.deltaTime;
                if (attackQueued && cooldownTimer > 0.1f)
                {
                    comboContinued = true;
                    PlayerAnimationManager.instance.SetComboAttackTrigger();
                    break;
                }
                yield return null;
            }
            //comboNum++;
            if (comboNum > 2)
            {
                //yield return new WaitForSeconds(0.5f);
                currentCombatState = CombatState.Idle;
                comboNum = 0;
                currentCombatCoroutine = null;
                break;
                
            }


                /*while (cooldownTimer < attackCooldownDurationSeconds)
            {
                if (attackQueued && cooldownTimer > 0.2f)
                {
                    attackQueued = false;
                    comboContinued = true;
                    PlayerAnimationManager.instance.SetComboAttackTrigger();
                    break;
                }
                cooldownTimer += Time.deltaTime;
                yield return null;
            }*/

            
        } while (attackQueued);





        //PlayerAnimationManager.instance.SetAttackQueued(false);
        comboNum = 0;
        currentCombatState = CombatState.Idle;
        currentCombatCoroutine = null;
    }

    /*public void CancelAttack()
    {
        PlayerAnimationManager.instance.SetAttackQueued(false);
        StopCoroutine(currentCombatCoroutine);
        currentCombatCoroutine = null;
        attackHitboxActive = false;
        attackHitbox.SetActive(false);
        PlayerAnimationManager.instance.enableSword();
        attackPressed = false;
        currentCombatState = CombatState.Idle;
        
    }*/

    //updates attack damage hitbox position to be in front of the player
    private void UpdateFacingDirection()
    {
        Vector3 playerPos = playerMovement.transform.position;
        Vector3 offsetVector = playerMovement.getFacingDirection() ? new Vector3(0.5f, 0, 0) : new Vector3(-0.5f, 0, 0);
        attackHitbox.transform.position = playerPos += offsetVector;
    }

    //called when the attack animation starts, begins execution of attack anim
    public void StartAttack()
    {

        /*if (playerMovement.getDashFrames() > 0)
        {
            return;
        }*/


        if (currentCombatCoroutine != null)
        {
            return;
        }
        

        /*if (currentCombatCoroutine != null)
        {
            attackHitbox.SetActive(false);
            attackHitboxActive = false;
            //StopCoroutine(currentCombatCoroutine);
            currentCombatCoroutine = null;
            comboNum = 0;
        }*/
        PlayerAnimationManager.instance.disableSword();
        PlayerAnimationManager.instance.SetSwingSwordTrigger();
        currentCombatCoroutine = StartCoroutine(AttackCoroutine());
    }

    
    

    //called from within the animation itself on prespecified "contact frames". Enables damage hitbox.
    /*public void ApplyDamage()
    {
        UpdateFacingDirection();
        
        attackHitbox.SetActive(true);
    }*/

    public void ApplyDashAttackDamage()
    {
        UpdateFacingDirection();
        attackHitbox.SetActive(true);
        PlayerHealthManager.instance.ShouldApplyDamage(false);
        
    }

    public void ExitDashAttack()
    {
        attackHitbox.SetActive(false);
        attackHitboxActive = false;
        PlayerHealthManager.instance.ShouldApplyDamage(true);
    }
}
