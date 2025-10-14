using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] float maxHealth;
    public float currentHealth { get; private set; }
    [SerializeField] private float iFrameDuration = 0.5f;
    private float iFrameTimer = 0;

    private void Awake()
    {
        if (maxHealth < 0) maxHealth = 5;
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        if (iFrameTimer > 0)
        {
            iFrameTimer -= Time.fixedDeltaTime;
        }
    }

    public float getMaxHealth()
    {
        return maxHealth;
    }

    public void ApplyDamageIgnoreIFrames(float amount)
    {
        currentHealth -= Mathf.Max(amount, 0);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ApplyDamage(float amount)
    {
        if (iFrameTimer <= 0)
        {
            ApplyDamageIgnoreIFrames(amount);
            iFrameTimer = iFrameDuration;
        }
    }

    public void ApplyDamage()
    {
        ApplyDamage(1);
    }

    public void RestoreAllHealth()
    {
        currentHealth = maxHealth;
    }

    public void Heal(float amount)
    {
        currentHealth += Mathf.Max(amount, 0);
    }

    public void Heal()
    {
        Heal(1);
    }

    public void Kill()
    {
        ApplyDamage(maxHealth);
    }

    public void Die()
    {
        print("died");
    }
}