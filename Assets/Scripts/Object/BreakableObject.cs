using UnityEngine;

public class BreakableObject : HealthManager
{

    public GameObject breakParticlesPrefab;

    public override void Die()
    {
        print("object broken");
        Destroy(this.gameObject);

        if (breakParticlesPrefab != null)
        {
            Instantiate(
                breakParticlesPrefab,
                transform.position,
                Quaternion.identity
            );
        }
    }
}
