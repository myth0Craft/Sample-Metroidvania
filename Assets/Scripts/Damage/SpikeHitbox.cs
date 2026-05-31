using UnityEngine;

public class SpikeHitbox : MonoBehaviour
{
    public int damageAmount;
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthManager health = other.GetComponent<PlayerHealthManager>();
            health.ApplySpikeDamage(damageAmount);
        }
    }
}
