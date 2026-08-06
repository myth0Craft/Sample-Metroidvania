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
    private Coroutine walkCoroutine;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        enemyMovementState = EnemyMovementState.Walk;

        //walkCoroutine = StartCoroutine(WalkCoroutine());
    }

    private void FixedUpdate()
    {
        body.linearVelocity = new Vector2(directionModifier * speed, body.linearVelocity.y);

        if (Physics2D.Raycast(
            wallCheck.position,
            Vector2.right * directionModifier,
            0.2f,
            LayerMask.GetMask("Ground")))
        {
            float xScale = gameObject.transform.localScale.x;
            gameObject.transform.localScale = new Vector3(xScale *= -1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
            directionModifier *= -1;
        }

        
    }

    private IEnumerator WalkCoroutine()
    {
        yield return null;
    }

    private IEnumerator StopCoroutine()
    {
        body.linearVelocity = new Vector2(0, body.linearVelocity.y);

        yield return new WaitForSeconds(Random.Range(stopDelayMin, stopDelayMax));
    }
    
}
