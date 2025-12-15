using UnityEngine;

public class BreakableObject : HealthManager
{

    public GameObject breakParticlesPrefab;

    public override void Die()
    {
        print("object broken");
        Destroy(this.gameObject);

        Instantiate(
            breakParticlesPrefab,
            transform.position,
            Quaternion.identity
        );

        Destroy(gameObject);

    }
}
