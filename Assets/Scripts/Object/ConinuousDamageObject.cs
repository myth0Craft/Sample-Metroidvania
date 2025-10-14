using UnityEngine;

public class ConinuousDamageObject : MonoBehaviour
{



    private void Awake()
    {

    }

    private void Update()
    {

    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager health = other.GetComponent<HealthManager>();
            health.ApplyDamage();
            print(health.currentHealth + "/" + health.getMaxHealth());
        }
    }
}
