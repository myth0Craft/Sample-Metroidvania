using UnityEngine;

public class Lever : MonoBehaviour
{
    private Animator anim;

    public bool isActivated = false;

    [SerializeField] private string id;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        if (id == null)
        {
            Debug.Log("Id of Lever is null!");
        }
        else
        {
            var room = SaveSystem.getRoom(gameObject.scene.name);

            if (room.pickups.TryGetValue(id, out bool activated) && activated)
            {
                isActivated = true;
            }
        }

        anim.SetBool("IsActivated", isActivated);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!isActivated)
        {
            if (collision.CompareTag("PlayerAttackHitbox"))
            {
                if (id == null)
                {
                    Debug.Log("Id of Lever is null!");
                }
                else
                {
                    var room = SaveSystem.getRoom(gameObject.scene.name);
                    room.pickups[id] = true;
                }

                CamShakeSource.instance.AddScreenShake(0.25f);

                anim.SetTrigger("Activate");
                isActivated = true;
            }
        }
    }
}
