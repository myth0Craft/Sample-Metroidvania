using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

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
        EnemyHealthManager enemyHealth = other.GetComponent <EnemyHealthManager>();
        if (health != null)
        {
            health.ApplyDamage();
            AddScreenShake(0.02f);
        }
        if (enemyHealth)
        {
            enemyHealth.ApplyDamage();
            AddScreenShake(0.02f);
            StartCoroutine(hitStopCoroutine());
        }
        //}
    }

    public void AddScreenShake(float amount)
    {
        float xForce = playerMovement.getFacingDirection() ? -amount : amount;
        if (playerMovement.getLinearVelocity().x > 0.01f || playerMovement.getLinearVelocity().x < -0.01f)
        {
            xForce *= 3;
        }
        Vector3 force = new Vector3(xForce, amount, 0);


        impulseSource.GenerateImpulse(force);
    }

    public IEnumerator hitStopCoroutine()
    {
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 1.0f;
    }
}
