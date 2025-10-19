using UnityEngine;
using System;

public class HealthManager : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    public int currentHealth { get; protected set; }
    protected float iFrameDuration = 0.5f;
    protected float iFrameTimer = 0;
    public event Action OnHealthChanged;

    private void Awake()
    {
        if (maxHealth < 0) maxHealth = 5;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (iFrameTimer > 0)
        {
            iFrameTimer = Mathf.Max(0f, iFrameTimer - Time.deltaTime);
        }
    }

    

    protected void HealthChanged()
    {
        OnHealthChanged?.Invoke();
    }

    public virtual float getMaxHealth()
    {
        return maxHealth;
    }

    public virtual void ApplyDamageIgnoreIFrames(int amount)
    {
        currentHealth -= Mathf.Max(amount, 0);
        HealthChanged();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ApplyDamage(int amount)
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
        HealthChanged();
    }

    public void Heal(int amount)
    {
        currentHealth += Mathf.Max(amount, 0);
        HealthChanged();
    }

    public void Heal()
    {
        Heal(1);
    }

    public void Kill()
    {
        ApplyDamage(maxHealth);
    }

    public virtual void Die()
    {
        print("died");
    }
}