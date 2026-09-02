using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{

    public PatrollerPathfinding pathfinding;

    public GameObject attackHitbox;
    public void EndAttack()
    {
        pathfinding.isAttacking = false;
    }

    public void EnableAttackHitbox()
    {
        attackHitbox.SetActive(true);
    }

    public void DisableAttackHitbox()
    {
        attackHitbox.SetActive(false);
    }
}
