using Unity.Cinemachine;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{

    //[SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private PlayerMeleeAttack playerAttack;

    [SerializeField] private AudioClip swordSwingSoundClip;

    public AudioClip footstep;

    private AudioSource audioSource;

    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void StartAttackCooldown()
    {
        PlayerMeleeAttack.instance.SetCombatState(CombatState.Cooldown);
    }

    public void SetDrawing()
    {
        PlayerMeleeAttack.instance.SetCombatState(CombatState.Drawing);
    }

    public void SetIdle()
    {
        PlayerMeleeAttack.instance?.SetCombatState(CombatState.Idle);
    }

    public void ExitDashAttack()
    {
        PlayerMovement.instance.OnSprintCanceled();
        PlayerMeleeAttack.instance.ExitDashAttack();
    }

    public void SetSwordDrawn()
    {
        PlayerMeleeAttack.instance.SwordDrawFinished();
    }

    public void SetSwordSheathed()
    {
        PlayerMeleeAttack.instance.swordDrawn = false;
        PlayerMeleeAttack.instance.comboNum = 0;
        SetIdle();
    }

    public void PlayFootstepSound()
    {
        if (PlayerMovement.instance.IsGroundedBuffered() && PlayerData.inWater == false)
        {
            audioSource.pitch = Random.Range(0.5f, 1.0f);
            audioSource.PlayOneShot(footstep, 0.1f);
        }
    }

    public void EnableSword()
    {
        PlayerAnimationManager.instance.enableSword();
    }

    public void DisableSword()
    {
        PlayerAnimationManager.instance.disableSword();
    }

    //called from 2nd sword swing animation to set player to mid attack state, preventing dashing during the sword swing
    public void PlayAttackSound()
    {
        //playerAttack.isMidAttack = true;
        AudioSource.PlayClipAtPoint(swordSwingSoundClip, PlayerMovement.instance.transform.position);
    }

    public void startOverheadSlash()
    {
        PlayerAnimationManager.instance.StartOverheadSlash();
        //PlayerAnimationManager.instance.SetAttackQueued(false);
    }

    public void triggerAttackScreenShake()
    {
        float xForce = PlayerMovement.instance.getFacingDirection() ? -0.008f : 0.008f;
        if (PlayerMovement.instance.getLinearVelocity().x > 0.01f || PlayerMovement.instance.getLinearVelocity().x < -0.01f)
        {
            xForce *= 3;
        }
        Vector3 force = new Vector3(xForce, 0.02f, 0);


        impulseSource.GenerateImpulse(force);

        //playerAttack.ApplyDamage();
    }

    public void StartDashAttack()
    {
        float xForce = PlayerMovement.instance.getFacingDirection() ? -0.1f : 0.1f;
        Vector3 force = new Vector3(xForce, 0.02f, 0);


        impulseSource.GenerateImpulse(force);
        
        playerAttack.ApplyDashAttackDamage();
    }

    public void ActivateBasicAttackHitbox()
    {
        PlayerMeleeAttack.instance.ActivateBasicAttackHitbox();
    }

    public void ActivateUpwardSlashHitbox()
    {
        PlayerMeleeAttack.instance.ActivateUpwardSlashHitbox();
    }

    public void ActivateDownwardSlashHitbox()
    {
        PlayerMeleeAttack.instance.ActivateDownwardSlashHitbox();
    }
}