using Unity.Cinemachine;
using UnityEngine;

public class PlayerAttackDamageObject : MonoBehaviour
{

    [SerializeField] PlayerMovement playerMovement;

    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        /*if (other.CompareTag("BreakableObj"))
        {*/
        BreakableObject health = other.GetComponent<BreakableObject>();
        if (health != null)
        {
            health.ApplyDamage();
            float xForce = playerMovement.getFacingDirection() ? -0.02f : 0.02f;
            if (playerMovement.getLinearVelocity().x > 0.01f || playerMovement.getLinearVelocity().x < -0.01f)
            {
                xForce *= 3;
            }
            Vector3 force = new Vector3(xForce, 0.02f, 0);


            impulseSource.GenerateImpulse(force);
        }
        //}
    }
}
