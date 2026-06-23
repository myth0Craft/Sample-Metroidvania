using UnityEngine;
using System.Collections;
public class PlatformWithVines : BreakableObject
{
    public Sprite spriteWithoutVines;
    private SpriteRenderer spriteRenderer;

    public float fallSpeed = 1f;

    public Vector2 fallPos;

    public override void Awake()
    {
        if (maxHealth < 0) maxHealth = 5;
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (saveState)
        {
            if (id == null)
            {
                Debug.Log("Id of Breakable Object is null!");
            }
            else
            {
                var room = SaveSystem.getRoom(gameObject.scene.name);

                if (room.breakables.TryGetValue(id, out bool broken) && broken)
                {
                    SetFallenPosition();
                }
            }
        }
    }

    public override void Die()
    {

        if (saveState)
        {
            if (id == null)
            {
                Debug.Log("Id of Breakable Object is null!");
            }
            else
            {
                var room = SaveSystem.getRoom(gameObject.scene.name);
                room.breakables[id] = true;
            }
        }
        


        if (breakParticlesPrefab != null && breakParticlesPrefab.GetComponent<ParticleSystem>() != null)
        {
            GameObject instance = Instantiate(breakParticlesPrefab, transform.position, Quaternion.identity);
            instance.GetComponent<ParticleSystem>().Play();
        }

        spriteRenderer.sprite = spriteWithoutVines;

        StartCoroutine(MoveCoroutine(transform.localPosition, fallPos));
    }

    


    private void SetFallenPosition()
    {
        transform.localPosition = fallPos;
        spriteRenderer.sprite = spriteWithoutVines;
    }

    private IEnumerator MoveCoroutine(Vector2 startPos, Vector2 endPos)
    {
        float elapsed = 0f;

        while (elapsed < fallSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fallSpeed;

            transform.localPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        CamShakeSource.instance.AddVerticalScreenShake(0.08f);

        transform.localPosition = endPos;
        

    }
}
