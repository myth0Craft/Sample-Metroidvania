using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float accel = 1f;
    public float speedIncrease = 0.5f;
    public float speed = 2f;
    public float maxSpeed = 20f;
    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = PlayerMovement.instance.transform.position;

        float xDistance = Mathf.Abs(currentPosition.x - targetPosition.x);
        float xDir = targetPosition.x < currentPosition.x ? -1f : 1f;

        float yDistance = Mathf.Abs(currentPosition.y - targetPosition.y);
        float yDir = targetPosition.y < currentPosition.y ? -1f : 1f;


        float angle = Mathf.Atan2((yDistance * yDir), (xDistance * xDir));

        float xVel = Mathf.Cos(angle);
        float yVel = Mathf.Sin(angle);

        body.linearVelocity = Vector2.Lerp(body.linearVelocity, new Vector2(xVel, yVel) * speed, Time.deltaTime * accel);
        if (speed < maxSpeed)
        {
            speed += speedIncrease;
        }

        if (speed > maxSpeed)
        {
            speed = maxSpeed;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPickUp();
        }
    }

    protected virtual void OnPickUp()
    {
        Destroy(gameObject);
    }

}
