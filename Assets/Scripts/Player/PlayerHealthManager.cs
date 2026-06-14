using System.Collections;
using UnityEngine;

public class PlayerHealthManager : HealthManager
{

    private SpriteRenderer[] spriteRenderers;
    public Material defaultMat;
    public Material shinyMat;
    public Material hurtMat;
    public ParticleSystem hitParticle;

    public AudioClip hurtSound;

    private bool isDead = false;

    private bool shouldApplyDamage = true;

    public static PlayerHealthManager instance;

    

    private void Awake()
    {
        this.iFrameDuration = 1.0f;

        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }

            isDead = false;
        hitParticle.enableEmission = false;
        this.maxHealth = PlayerData.maxHealth;
        this.currentHealth = PlayerData.currentHealth;
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void ShouldApplyDamage(bool shouldApplyDamage)
    {
        this.shouldApplyDamage = shouldApplyDamage;
    }

    public override void ApplyDamageIgnoreIFrames(int amount)
    {
        if (PlayerMeleeAttack.instance.currentCombatState == CombatState.Blocking)
        {
            PlayerMeleeAttack.instance.AddBlockEffects();
            return;
        }


        if (shouldApplyDamage)
        {
            base.ApplyDamageIgnoreIFrames(amount);
            print(maxHealth + "/" + currentHealth);
        }
        
    }

    public void StopDamageForDuration(float durationSeconds)
    {
        StartCoroutine(StopDamageForDurationCoroutine(durationSeconds));
    }

    private IEnumerator StopDamageForDurationCoroutine(float durationSeconds)
    {
        shouldApplyDamage = false;
        yield return new WaitForSeconds(durationSeconds);
        shouldApplyDamage = true;
    }

    public override void ApplyDamage(int amount)
    {
        if (iFrameTimer <= 0 && shouldApplyDamage)
        {
            ApplyDamageIgnoreIFrames(amount);
            iFrameTimer = iFrameDuration;
        }
    }

    public void ApplySpikeDamage(int amount)
    {
        base.ApplyDamageIgnoreIFrames(amount);
        iFrameTimer = iFrameDuration;
        if (currentHealth > 0)
        {
            SafeZoneTracker.instance.MoveParentToLastSafeZone();
            StartCoroutine(DisableInputOnHurt());
        }
        
    }

    private IEnumerator DisableInputOnHurt()
    {
        PlayerData.AllowGameInput(false);
        yield return new WaitForSeconds(0.25f);
        PlayerData.AllowGameInput(true);
    }

    public override void Die()
    {
        if (!isDead)
        {
            isDead = true;
            StopAllCoroutines();
            for (int x = 0; x < spriteRenderers.Length; x++)
            {
                spriteRenderers[x].material = defaultMat;
            }
            hitParticle.enableEmission = false;



            SceneLoader.instance.LoadGameFromPlayerDeath();
        }
    }

    /*private IEnumerator DeathCoroutine()
    {
        
        
        
        
        //yield return SceneLoader.instance.UnloadAllScenes();
        yield return SceneLoader.instance.LoadGameFromPlayerDeath();
    }*/

    protected override void AddHitEffects()
    {
        AudioSource.PlayClipAtPoint(hurtSound, PlayerMovement.instance.transform.position, 10.0f);
        CamShakeSource.instance.AddScreenShake(0.05f);
        StartCoroutine(HitColorCoroutine());
        StartCoroutine(hitParticles());
        GlobalHitstopManager.DoHitstop(0.05f);
        

        //StartCoroutine(hitStopCoroutine());
    }

    //public IEnumerator hitStopCoroutine()
    //{
    //    Time.timeScale = 0.0f;
    //    yield return new WaitForSecondsRealtime(0.05f);
    //    Time.timeScale = 1.0f;
    //}

    public IEnumerator hitParticles()
    {
        hitParticle.enableEmission = true;
        yield return new WaitForSecondsRealtime(0.5f);
        hitParticle.enableEmission = false;
    }

    public IEnumerator HitColorCoroutine()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int x = 0; x < spriteRenderers.Length; x++)
            {
                spriteRenderers[x].material = hurtMat;
            }

            yield return new WaitForSecondsRealtime(0.15f);
            for (int x = 0; x < spriteRenderers.Length; x++)
            {
                spriteRenderers[x].material = defaultMat;
            }
            yield return new WaitForSecondsRealtime(0.15f);
        }
        for (int i = 0; i < 4; i++)
        {
            for (int x = 0; x < spriteRenderers.Length; x++)
            {
                spriteRenderers[x].material = hurtMat;
            }

            yield return new WaitForSecondsRealtime(0.075f);
            for (int x = 0; x < spriteRenderers.Length; x++)
            {
                spriteRenderers[x].material = defaultMat;
            }
            yield return new WaitForSecondsRealtime(0.075f);
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i].gameObject.name == "Body" || spriteRenderers[i].gameObject.name == "Sword")
            {
                spriteRenderers[i].material = shinyMat;
            }
        }
    }
}