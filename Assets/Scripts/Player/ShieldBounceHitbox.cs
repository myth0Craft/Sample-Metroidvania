
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ShieldBounceHitbox : MonoBehaviour
{

    public GameObject sparkParticles;
    private GameObject currentSparkInstance;

    private CinemachineImpulseSource impulseSource;

    private CamShakeSource camShakeSource;

    private bool isShieldBouncing = false;
    private bool hitWhileShieldBouncing = false;

    public static ShieldBounceHitbox instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        camShakeSource = GameObject.FindGameObjectWithTag("CinemachineImpulseSource").GetComponent<CamShakeSource>();
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        /*if (other.CompareTag("BreakableObj"))
        {*/
        if (isShieldBouncing)
        {
            BreakableObject health = other.GetComponent<BreakableObject>();
            EnemyHealthManager enemyHealth = other.GetComponent<EnemyHealthManager>();
            if (health != null)
            {
                health.ApplyDamage();
                CamShakeSource.instance.AddScreenShake(0.08f);

                isShieldBouncing = false;
                hitWhileShieldBouncing = true;


            }
            if (enemyHealth != null)
            {
                enemyHealth.ApplyDamage();
                CamShakeSource.instance.AddScreenShake(0.08f);

                GlobalHitstopManager.DoHitstop(0.05f);
                //StartCoroutine(hitStopCoroutine());
                if (currentSparkInstance != null)
                {
                    Destroy(currentSparkInstance.gameObject);
                }

                currentSparkInstance = Instantiate(
                    sparkParticles,
                    transform.position,
                    Quaternion.identity
                );
                StartCoroutine(DestroySparkParticleCoroutine());

                isShieldBouncing = false;
                hitWhileShieldBouncing = true;
                

            }
        }

        
        //}
    }

    public IEnumerator DestroySparkParticleCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(currentSparkInstance.gameObject);
    }

    public IEnumerator DoShieldBounce()
    {
        gameObject.SetActive(true);
        isShieldBouncing = true;
        float shieldBounceTime = 0.25f;
        float timeElapsed = 0;

        while (timeElapsed < shieldBounceTime)
        {
            timeElapsed += Time.deltaTime;
            if (hitWhileShieldBouncing)
            {

                isShieldBouncing = false;
                hitWhileShieldBouncing = false;
                //PlayerHealthManager.instance.StopDamageForDuration(0.2f);
                PlayerMovement.instance.ApplyShieldBounceForce();
                break;
            }
            yield return null;
        }

        gameObject.SetActive(false);
    }



    //public IEnumerator hitStopCoroutine()
    //{
    //    Time.timeScale = 0.0f;
    //    yield return new WaitForSecondsRealtime(0.05f);
    //    Time.timeScale = 1.0f;
    //}
}


