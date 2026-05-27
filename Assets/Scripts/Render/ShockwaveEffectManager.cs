using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ShockwaveEffectManager : MonoBehaviour
{
   public static ShockwaveEffectManager instance;

    public Material shockwaveMaterial;

    private Coroutine currentShockwaveCoroutine;

    private float speed = 2f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void StartShockwave()
    {
        StartShockwave(0.1f, -0.1f, new Vector2(0.5f, 0.5f));
    }

    public void StartShockwave(Vector2 spawnPosition)
    {
        StartShockwave(0.1f, -0.1f, spawnPosition);
    }

    public void StartShockwave(float size, float waveStrength, Vector2 spawnPosition)
    {
        if (currentShockwaveCoroutine != null)
        {
            StopCoroutine(currentShockwaveCoroutine);
        }
        shockwaveMaterial.SetFloat("_Size", size);
        shockwaveMaterial.SetFloat("_WaveStrength", waveStrength);
        shockwaveMaterial.SetFloat("_WaveDistanceFromCenter", -0.1f);
        shockwaveMaterial.SetVector("_RingSpawnPosition", spawnPosition);
        shockwaveMaterial.SetFloat("_GlobalAlpha", 1f);
        currentShockwaveCoroutine = StartCoroutine(ShockwaveCoroutine());
    }

    private IEnumerator ShockwaveCoroutine()
    {
        float distanceFromCenter = -0.1f;

        while (distanceFromCenter < 1f)
        {
            distanceFromCenter += Time.deltaTime * speed;

            shockwaveMaterial.SetFloat("_WaveDistanceFromCenter", distanceFromCenter);

            yield return null;
        }

        shockwaveMaterial.SetFloat("_GlobalAlpha", 0f);
    }
}
