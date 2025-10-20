using UnityEngine;

public class PlayerAttackDamageObject : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        /*if (other.CompareTag("BreakableObj"))
        {*/
            BreakableObject health = other.GetComponent<BreakableObject>();
        if (health != null)
        {
            health.ApplyDamage();
            print(health.currentHealth + "/" + health.getMaxHealth());
            print("attacked breakable");
        } else
        {
            print("health null");
        }
        //}
    }
}
