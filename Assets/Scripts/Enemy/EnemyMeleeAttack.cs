using System.Collections;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [SerializeField] Animator animator;

    public float minTimeBetweenAttacks = 1f;

    public float maxTimeBetweenAttacks = 2f;

    public float attackHitboxDuration;
    public float totalAttackDuration;

    private Coroutine currentAttackCoroutine;

    private BoxCollider2D attackHitbox;

    private void Awake()
    {
        attackHitbox = GetComponent<BoxCollider2D>();
        attackHitbox.enabled = false;
    }

    private void Update()
    {
        float time = 0;
        if (animator != null && time % 15 == 0) 
        {
            animator.SetTrigger("Attack");
        }

        time++;
    }


    private IEnumerator AttackCoroutine()
    {
        yield return null;
    }
}
