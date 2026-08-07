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

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        enemyMovementState = EnemyMovementState.Walk;
        if (stopDelay)
        {
            StartCoroutine(StopCoroutineController());
        }
        

    }

    private void FixedUpdate()
    {
        if (stopCoroutine != null) return;

        //Dont turn if falling
        if (body.linearVelocityY < -0.1f)
        {
            return;
        }


        body.linearVelocity = new Vector2(directionModifier * speed, body.linearVelocity.y);
        

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
            Stop();
        }
    }
    
}
