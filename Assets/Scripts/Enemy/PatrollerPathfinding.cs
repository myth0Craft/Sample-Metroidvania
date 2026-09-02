using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PatrollerPathfinding : MonoBehaviour
{
    public enum EnemyMovementState
    {
        Idle,
        Walk
    }

    private Rigidbody2D body;

    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private PlayerDetector playerDetector;
    [SerializeField] private PlayerDetector playerAttackDetector;

    [SerializeField] private Animator animator;

    public bool isAttacking = false;

    private Coroutine attackCoroutine;

    public float speed;

    public float speedModifier = 1;

    private float directionModifier = 1;

    public EnemyMovementState enemyMovementState;

    public bool stopDelay;

    public float stopDelayMin;
    public float stopDelayMax;

    public float minTimeBeforeStopping;
    public float maxTimeBeforeStopping;

    private Coroutine stopCoroutine;

    private bool attackAllowed = true;

    public float attackCooldown = 1.0f;

    private Coroutine attackCooldownCoroutine;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        enemyMovementState = EnemyMovementState.Walk;
        if (stopDelay)
        {
            StartCoroutine(StopCoroutineController());
        }
    }

    private void Update()
    {
        if (Mathf.Abs(body.linearVelocity.x) > 0.1f)
        {
            animator.SetBool("Walking", true);
        } else
        {
            animator.SetBool("Walking", false);
        }
    }
    private void FixedUpdate()
    {
        if (stopCoroutine != null) return;

        if (attackCoroutine != null) return;

        //Dont turn if falling
        if (body.linearVelocityY < -0.1f)
        {
            return;
        }

        if (PlayerInAttackRange())
        {
            Attack();
            return;
        }

        if (Mathf.Abs(PlayerMovement.instance.transform.position.x - transform.position.x) < 0.5f)
        {
            return;
        }

        

        float finalSpeedModifier = PlayerInRange() ? speedModifier : 1;

        body.linearVelocity = new Vector2(directionModifier * speed * finalSpeedModifier, body.linearVelocity.y);
        
        if (PlayerInRange())
        {
            if (PlayerMovement.instance.gameObject.transform.position.x < transform.position.x)
            {
                if (!FacingRight())
                {
                    Turn();
                }
            }
            else if (PlayerMovement.instance.gameObject.transform.position.x > transform.position.x)
            {
                if (FacingRight())
                {
                    Turn();
                }
            }

            return;
        }

        


        //check for wall in front
        if (Physics2D.Raycast(
            wallCheck.position,
            Vector2.right * directionModifier,
            0.2f,
            LayerMask.GetMask("Ground")))
        {
            Turn();
        }

        //check for ledges
        if (!(Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            0.2f,
            LayerMask.GetMask("Ground")))) 
        {
            Turn();
        }
    }

    private void Turn()
    {
        float xScale = gameObject.transform.localScale.x;
        gameObject.transform.localScale = new Vector3(xScale *= -1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        directionModifier *= -1;
    }

    private void Stop()
    {
        stopCoroutine = StartCoroutine(StopCoroutine());
    }

    private IEnumerator StopCoroutine()
    {
        body.linearVelocity = new Vector2(0, body.linearVelocity.y);

        yield return new WaitForSeconds(Random.Range(stopDelayMin, stopDelayMax));

        EndStopCoroutine();

    }

    private void EndStopCoroutine()
    {
        StopCoroutine(stopCoroutine);
        stopCoroutine = null;
        if (UnityEngine.Random.value > 0.5f)
        {
            Turn();
        }
    }

    private IEnumerator StopCoroutineController()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBeforeStopping, maxTimeBeforeStopping));
            while (PlayerInRange())
            {
                yield return null;
            }
            Stop();
        }
    }

    public IEnumerator AttackCooldownCoroutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        attackAllowed = true;
        attackCooldownCoroutine = null;
    }

    //ghost knight capabilities

    private void Attack()
    {
        if (attackCooldownCoroutine != null) return;
        if (attackCoroutine != null) return;

        animator.SetTrigger("Attack");
        isAttacking = true;

        if (attackAllowed)
        {
            attackCoroutine = StartCoroutine(AttackCoroutine());
        } else
        {
            attackCooldownCoroutine = StartCoroutine(AttackCooldownCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        while (isAttacking == true)
        {
            yield return null;
        }

        attackCoroutine = null;
        attackCooldownCoroutine = StartCoroutine(AttackCooldownCoroutine());
    }

    private bool PlayerInRange()
    {
        return playerDetector.playerInRange;
    }

    private bool PlayerInAttackRange()
    {
        return playerAttackDetector.playerInRange;
    }

    private bool FacingRight()
    {
        return directionModifier == 1;
    }
}