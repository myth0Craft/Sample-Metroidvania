using System.Collections;
using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    public static StaminaManager instance;
    [SerializeField] private int currentStamina = 100;
    [SerializeField] private int maxStamina = 100;

    [SerializeField] private float cooldownBeforeRecharge = 5f;

    private bool rechargeIsOnCooldown = false;

    private int framesElapsed = 0;
    [SerializeField] private int rechargeSpeed = 30;
    [SerializeField] private Material staminaMeterMat;

    private Coroutine currentCooldown;

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

    public void RestoreStamina(int amount)
    {
        currentStamina += amount;
        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
    }

    public void DecrementStamina(int amount)
    {

        currentStamina = currentStamina - amount;

        if (currentStamina <= 0) {
            currentStamina = 0;
           
        }
        if (currentCooldown != null)
        {
            currentCooldown = null;
        }
        currentCooldown = StartCoroutine(CooldownBeforeStaminaRechargeCoroutine());
    }

    public bool CanAffordStaminaCost(int amount)
    {
        if (currentStamina >= amount)
        {
            return true;
        } else 
            return false;
    }

    private IEnumerator CooldownBeforeStaminaRechargeCoroutine()
    {
        rechargeIsOnCooldown = true;
        yield return new WaitForSeconds(cooldownBeforeRecharge);
        rechargeIsOnCooldown = false;
    }

    private void Update()
    {
        framesElapsed++;
        if (framesElapsed > 1000)
        {
            framesElapsed = 0;
        }

        if (!rechargeIsOnCooldown && framesElapsed % rechargeSpeed == 0)
        {
            RestoreStamina(1);
        }

        staminaMeterMat.SetFloat("_Stamina", (float)currentStamina / (float)maxStamina);
    }
}
