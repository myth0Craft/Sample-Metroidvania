using System.Collections;
using UnityEngine;

public class GrassExternalVelocityTrigger : MonoBehaviour
{
    private GrassSwayController controller;

    private GameObject player;

    private Material material;

    private Rigidbody2D playerBody;

    private bool easeInCoroutineRunning;
    private bool easeOutCoroutineRunning;

    private int externalInfluence = Shader.PropertyToID("_externalInfluence");

    private float startingXVelocity;

    private float velocityLastFrame;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        playerBody = player.GetComponent<Rigidbody2D>();

        controller = GetComponentInParent<GrassSwayController>();

        material = GetComponent<SpriteRenderer>().material;

        startingXVelocity = material.GetFloat(externalInfluence);

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {

            if (!easeInCoroutineRunning && Mathf.Abs(playerBody.linearVelocity.x) > Mathf.Abs(controller.VelocityThreshold))
            {
                StartCoroutine(EaseIn(playerBody.linearVelocity.x * controller.ExternalInfluenceStrength));
                print("started coroutine");
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            StartCoroutine(EaseOut());
            print("easing out coroutine");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {

            if (Mathf.Abs(velocityLastFrame) > Mathf.Abs(controller.VelocityThreshold) && 
                Mathf.Abs(playerBody.linearVelocity.x) < Mathf.Abs(controller.VelocityThreshold))
            {
                StartCoroutine(EaseOut());
            } else if (Mathf.Abs(velocityLastFrame) < Mathf.Abs(controller.VelocityThreshold) &&
                Mathf.Abs(playerBody.linearVelocity.x) > Mathf.Abs(controller.VelocityThreshold))
            {
                StartCoroutine(EaseIn(playerBody.linearVelocityX * controller.ExternalInfluenceStrength));
            } else if (!easeInCoroutineRunning && !easeOutCoroutineRunning && 
                Mathf.Abs(playerBody.linearVelocity.x) > Mathf.Abs(controller.VelocityThreshold))
            {
                controller.InfluenceGrass(material, playerBody.linearVelocity.x * controller.ExternalInfluenceStrength);
            }





                velocityLastFrame = playerBody.linearVelocity.x;
        }
    }


    private IEnumerator EaseIn(float XVelocity)
    {
        easeInCoroutineRunning = true;

        float elaspedTime = 0f;

        while(elaspedTime < controller.EaseInTime)
        {
            elaspedTime += Time.deltaTime;

            float lerpedAmount = Mathf.Lerp(startingXVelocity, XVelocity, (elaspedTime / controller.EaseInTime));
            controller.InfluenceGrass(material, lerpedAmount);

            yield return null;
        }

        easeInCoroutineRunning = false;
    }

    private IEnumerator EaseOut()
    {
        easeOutCoroutineRunning = true;

        float currentXInfluence = material.GetFloat(externalInfluence);

        float elaspedTime = 0f;

        while (elaspedTime < controller.EaseOutTime)
        {
            elaspedTime += Time.deltaTime;

            float lerpedAmount = Mathf.Lerp(currentXInfluence, startingXVelocity, (elaspedTime / controller.EaseOutTime));
            controller.InfluenceGrass(material, lerpedAmount);

            yield return null;
        }

        easeOutCoroutineRunning = false;
    }
}
