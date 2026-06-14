using System.Collections;
using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    public static StaminaManager instance;
    [SerializeField] private float currentStamina = 100;
    [SerializeField] private float maxStamina = 100;

    [SerializeField] private float cooldownBeforeRecharge = 5f;

    private bool rechargeIsOnCooldown = false;

    [SerializeField] private float rechargeFrequency = 10;
    [SerializeField] private Material staminaMeterMat;

    private Coroutine currentCooldown;

    [SerializeField] private ParticleSystem staminaMeterFullParticles;

    [SerializeField] private ParticleSystem staminaMeterEmptyParticles;

    public float lowStaminaThreshold = 40f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public void RestoreStamina(float amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);

        if (currentStamina == maxStamina)
        {
            staminaMeterFullParticles.Play();
        }
    }


    public void DecrementStamina(float amount)
    {

        currentStamina = Mathf.Max(currentStamina - amount, 0f);

        if (currentStamina <= lowStaminaThreshold)
        {
            staminaMeterEmptyParticles.Play();
        }

        if (currentCooldown != null)
        {
            StopCoroutine(currentCooldown);
        }
        currentCooldown = StartCoroutine(CooldownBeforeStaminaRechargeCoroutine());
    }

    public bool CanAffordStaminaCost(float amount)
    {
        if (currentStamina >= amount)
        {
            return true;
        } else 
            return false;
    }

    private IEnumerator CooldownBeforeStaminaRechargeCoroutine()
    {
        Debug.Log("Stamina recharge on cooldown");
        rechargeIsOnCooldown = true;
        yield return new WaitForSeconds(cooldownBeforeRecharge);
        rechargeIsOnCooldown = false;
    }

    private void Update()
    {

        if (!rechargeIsOnCooldown && currentStamina < maxStamina)
        {
            RestoreStamina(Time.deltaTime * rechargeFrequency);
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        staminaMeterMat.SetFloat("_Stamina", currentStamina / maxStamina);

        if (currentStamina < maxStamina)
        {
            staminaMeterFullParticles.Stop();
        }

        if (!(currentStamina <= lowStaminaThreshold))
        {
            staminaMeterEmptyParticles.Stop();
        }

    }
}
