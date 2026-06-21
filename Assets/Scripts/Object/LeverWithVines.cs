using UnityEngine;

public class LeverWithVines : BreakableObject
{

    private Animator anim;
    private Lever lever;

    public override void Awake()
    {
        if (maxHealth < 0) maxHealth = 5;
        currentHealth = maxHealth;

        anim = GetComponent<Animator>();
        lever = GetComponent<Lever>();

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
                    EnableLever();
                } else
                {
                    DisableLever();
                }
            }
        }
    }

    private void EnableLever()
    {
        anim.SetBool("HasVines", false);
        lever.canTrigger = true;
    }

    private void DisableLever()
    {
        anim.SetBool("HasVines", true);
        lever.canTrigger = false;
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

        EnableLever();
    }
}
