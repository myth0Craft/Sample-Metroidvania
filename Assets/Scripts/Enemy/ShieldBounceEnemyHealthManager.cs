using System.Collections;
using UnityEngine;

public class ShieldBounceEnemyHealthManager : EnemyHealthManager
{
    //private BoxCollider2D collider;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hurtSprite;

    public float timeToRevive = 1.5f;

    public override void Die()
    {

        if (shouldSaveAcrossRooms)
        {
            if (id == null)
            {
                Debug.Log("Id of Enemy is null!");
            }
            else
            {
                var room = SaveSystem.getRoom(gameObject.scene.name);
                //room.breakables[id] = true;
            }
        }

        print("enemy killed");
        AddParticles(deathParticlesPrefab);
        //Destroy(transform.parent.gameObject);
        StartCoroutine(RemoveHitboxOnDeath());

    }

    private IEnumerator RemoveHitboxOnDeath()
    {
        spriteRenderer.sprite = hurtSprite;
        hitCollider.enabled = false;
        yield return new WaitForSeconds(timeToRevive);
        hitCollider.enabled = true;
        spriteRenderer.sprite = defaultSprite;
    }
}
