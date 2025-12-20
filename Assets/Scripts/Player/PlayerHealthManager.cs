using UnityEngine;

public class PlayerHealthManager : HealthManager
{
    
    private void Awake()
    {
        this.maxHealth = PlayerData.maxHealth;
        this.currentHealth = PlayerData.currentHealth;
    }

    public override void ApplyDamageIgnoreIFrames(int amount)
    {
        base.ApplyDamageIgnoreIFrames(amount);
        print(maxHealth + "/" + currentHealth);
    }

    public override void Die()
    {
        print("died");
    }
}