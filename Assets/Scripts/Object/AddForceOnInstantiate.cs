using UnityEngine;

public class AddForceOnInstantiate : MonoBehaviour
{
    public float xForce = 1f;
    public float yForce = 1f;

    public bool randomizeForce = false;
    public bool randomizeRotation = true;
    public bool randomizeScale = false;


    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        if (randomizeRotation)
        {
            body.rotation = Random.Range(0f, 360f);
        }

        if (randomizeScale)
        {
            float randomizedScale = Random.Range(1f, 1.5f);
            transform.localScale = new Vector3(randomizedScale, randomizedScale, 1f);
        }

        var xDir = PlayerMovement.instance.getFacingDirection() ? 1 : -1;

        if (randomizeForce)
        {
            body.AddForce(new Vector2(xDir * xForce * Random.Range(0.5f, 1.5f), yForce * Random.Range(0.5f, 1.5f)));
        } else
        {
            body.AddForce(new Vector2(xDir * xForce, yForce));
        }

        
    }
}
