using UnityEngine;

public class DamageObject : MonoBehaviour
{
    private void Awake()
    {
        
    }

    private void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager health = other.GetComponent<HealthManager>();
            health.ApplyDamage();
            print(health.currentHealth + "/" + health.getMaxHealth());
        }
    }
}
