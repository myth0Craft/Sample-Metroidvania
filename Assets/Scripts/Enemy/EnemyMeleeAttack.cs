using System.Collections;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [SerializeField] Animator animator;

    public float minTimeBetweenAttacks = 1f;

    public float maxTimeBetweenAttacks = 2f;

    public float timeUntilHitboxActivates;
    public float attackHitboxDuration;
    public float totalAttackDuration;

    private Coroutine currentAttackCoroutine;

    public GameObject attackHitbox;

    private void Awake()
    {
        attackHitbox.SetActive(false);
    }

    private void Update()
    {
        if (currentAttackCoroutine == null)
        {
            currentAttackCoroutine = StartCoroutine(AttackCoroutine());
        }
    }


    private IEnumerator AttackCoroutine()
    {
        animator.SetTrigger("Attack");

        
        yield return new WaitForSeconds(timeUntilHitboxActivates);

        attackHitbox.SetActive(true);
        yield return new WaitForSeconds(attackHitboxDuration);
        attackHitbox.SetActive(false);

        yield return new WaitForSeconds(totalAttackDuration - attackHitboxDuration - timeUntilHitboxActivates);
        yield return new WaitForSeconds(Random.Range(minTimeBetweenAttacks, maxTimeBetweenAttacks));
        currentAttackCoroutine = null;
    }
}
