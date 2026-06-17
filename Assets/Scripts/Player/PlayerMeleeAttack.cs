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

    private float attackCooldownDurationSeconds = 0.8f;
    [SerializeField] private GameObject attackHitbox;

    [SerializeField] private bool attackDebug;
    public bool attackDebugActive { get; private set; } = false;
    private BoxCollider2D attackCollider;

    public int comboNum = 0;

    private float attackStartupSeconds = 0.28f;

    private bool attackQueued = false;

    private Coroutine currentCombatCoroutine;

    public int combatStaminaCost = 15;
    public int dashAttackStaminaCost = 30;
    public float blockStaminaCost = 20;
    public float staminaRequiredToBlock = 10;
    public float amountOfStaminaRestoredOnParry = 20;

    public GameObject blockParticle;

    private bool successfullyParried = false;

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
    public void ResetCombatState()
    {
        if (currentCombatCoroutine != null)
        {
            StopCoroutine(currentCombatCoroutine);
        }
        
        currentCombatCoroutine = null;
        currentCombatState = CombatState.Idle;
        comboNum = 0;
        attackHitbox.SetActive(false);
        attackHitboxActive = false;
        PlayerAnimationManager.instance.enableSword();
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
    }

    void OnDisable()
    {
        if (controls != null)
        {
            controls.Player.Disable();
        }
        controls.Player.Attack.performed -= OnAttackPressed;
    }

    public void OnBlock()
    {
        if (currentCombatState == CombatState.Startup || currentCombatState == CombatState.Active || currentCombatState == CombatState.Blocking) return;

        if (!StaminaManager.instance.CanAffordStaminaCost(staminaRequiredToBlock)) return;


        if (currentCombatState == CombatState.Cooldown || currentCombatState == CombatState.Active)
        {
            ResetCombatState();
        }


        successfullyParried = false;
        PlayerAnimationManager.instance.Block();
        currentCombatState = CombatState.Blocking;
        StartCoroutine(BlockCoroutine());
    }

    private IEnumerator BlockCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        currentCombatState = CombatState.Idle;
        if (!successfullyParried)
        {
            StaminaManager.instance.DecrementStamina(blockStaminaCost);
        }
        successfullyParried = false;
        ResetCombatState();
    }

    public void AddBlockEffects()
    {
        successfullyParried = true;
        StaminaManager.instance.RestoreStamina(amountOfStaminaRestoredOnParry);
        Instantiate(blockParticle, transform.position, Quaternion.identity);
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        //GlobalHitstopManager.DoHitstop(0.05f);
        GlobalHitstopManager.DoHitStopThenHitSlow(0.05f, 0.15f, 0.25f);
        CamShakeSource.instance.AddScreenShake(0.2f);

        ShockwaveEffectManager.instance.SetSpeed(3f);
        ShockwaveEffectManager.instance.StartShockwave(new Vector2(viewportPos.x, viewportPos.y));
    }

    private void OnAttackPressed(InputAction.CallbackContext context)
    {


        if ((PlayerData.swordUnlocked || attackDebugActive) && !PlayerData.gamePaused)
        {
            if (!StaminaManager.instance.CanAffordStaminaCost(combatStaminaCost))
            {
                return;
            }

            if (comboNum >= 2)
            {
                return;
            }


            //Debug.Log("Clicked with combat state " + currentCombatState + " at combo stage " + comboNum);
            if (playerMovement.currentHorizontalState == HorizontalState.Dashing && playerMovement.IsGroundedBuffered()
                )
            {
                if (!StaminaManager.instance.CanAffordStaminaCost(dashAttackStaminaCost)) return;

                PerformDashAttack();
                return;
            }

            if (!StaminaManager.instance.CanAffordStaminaCost(combatStaminaCost)) return;

            

            if (currentCombatState == CombatState.Idle)
            {
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
        StaminaManager.instance.DecrementStamina(dashAttackStaminaCost);
        PlayerAnimationManager.instance.LungeAttack();
        PlayerAnimationManager.instance.disableSword();
        PlayerMovement.instance.currentHorizontalState = HorizontalState.Dashing;
        PlayerMovement.instance.OnDashAttack();
    }


    private IEnumerator AttackCoroutine()
    {
        comboNum = 0;
        do
        {
            StaminaManager.instance.DecrementStamina(combatStaminaCost);
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

            while (cooldownTimer < attackCooldownDurationSeconds)
            {
                cooldownTimer += Time.deltaTime;
                if (attackQueued && cooldownTimer > 0.1f)
                {
                    PlayerAnimationManager.instance.SetComboAttackTrigger();
                    break;
                }
                yield return null;
            }

            if (comboNum > 2)
            {
                currentCombatState = CombatState.Idle;
                comboNum = 0;
                currentCombatCoroutine = null;
                break;
                
            }
        } while (attackQueued);

        comboNum = 0;
        currentCombatState = CombatState.Idle;
        currentCombatCoroutine = null;
    }

    /*public void CancelAttack()
    {
        StopCoroutine(currentCombatCoroutine);
        currentCombatCoroutine = null;
        attackHitboxActive = false;
        attackHitbox.SetActive(false);
        PlayerAnimationManager.instance.enableSword();
        currentCombatState = CombatState.Idle;
    }*/

    //updates attack damage hitbox position to be in front of the player
    private void UpdateFacingDirection()
    {
        Vector3 playerPos = playerMovement.transform.position;

        float multiplier;

        if (playerMovement.currentVerticalState == VerticalState.StuckToWall)
        {
            multiplier = -1f;
        } else
        {
            multiplier = 1f;
        }

        Vector3 offsetVector = playerMovement.getFacingDirection() ? new Vector3(0.5f * multiplier, 0, 0) : new Vector3(-0.5f * multiplier, 0, 0);
        attackHitbox.transform.position = playerPos += offsetVector;
    }

    //called when the attack animation starts, begins execution of attack anim
    public void StartAttack()
    {
        if (currentCombatCoroutine != null)
        {
            return;
        }

        

        PlayerAnimationManager.instance.disableSword();
        PlayerAnimationManager.instance.SetSwingSwordTrigger();
        currentCombatCoroutine = StartCoroutine(AttackCoroutine());
    }

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