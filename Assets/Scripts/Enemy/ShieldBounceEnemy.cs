using System.Runtime.CompilerServices;
using UnityEngine;

public class ShieldBounceEnemy : MonoBehaviour
{
    public bool shouldBobUpAndDown = false;
    public float maxDistance = 1f;
    public float bobSpeed = 1f;
    private bool goingDown = true;
    private float startYPos;

    public void Awake()
    {
        startYPos = transform.position.y;
        goingDown = UnityEngine.Random.Range(0, 2) == 1;
        startYPos += UnityEngine.Random.Range(-maxDistance, maxDistance);
    }

    private void Update()
    {
        if (shouldBobUpAndDown)
        {
            if (goingDown)
            {
                float y = Mathf.Lerp(transform.position.y, transform.position.y - maxDistance, Time.deltaTime * bobSpeed);
                


                transform.SetPositionAndRotation(new Vector3(transform.position.x, y, 0), Quaternion.identity);

                if (transform.position.y < startYPos - maxDistance)
                {
                    goingDown = false;
                }
            } else
            {
                float y = Mathf.Lerp(transform.position.y, transform.position.y + maxDistance, Time.deltaTime * bobSpeed);



                transform.SetPositionAndRotation(new Vector3(transform.position.x, y, 0), Quaternion.identity);

                if (transform.position.y > startYPos + maxDistance)
                {
                    goingDown = true;
                }
            }
        }
    }
}
