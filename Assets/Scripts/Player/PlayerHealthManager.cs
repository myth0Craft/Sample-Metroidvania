using UnityEngine;

public class PlayerHealthManager : HealthManager
{
    
    private void Awake()
    {
        this.maxHealth = PlayerDataManager.Instance.Data.maxHealth;
        this.currentHealth = PlayerDataManager.Instance.Data.currentHealth;
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