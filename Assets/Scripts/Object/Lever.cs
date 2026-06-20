using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    private Animator anim;

    public bool isActivated = false;

    public bool onlyWorksOnce = false;

    [SerializeField] private string id;

    public List<Gate> gates = new List<Gate>();

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

        if ((!isActivated && onlyWorksOnce) || !onlyWorksOnce)
        {
            if (collision.CompareTag("PlayerAttackHitbox"))
            {

                isActivated = !isActivated;

                if (id == null)
                {
                    Debug.Log("Id of Lever is null!");
                }
                else
                {
                    var room = SaveSystem.getRoom(gameObject.scene.name);
                    room.pickups[id] = isActivated;
                }

                CamShakeSource.instance.AddScreenShake(0.25f);
                anim.SetBool("Locked", onlyWorksOnce);
                anim.SetBool("IsActivated", isActivated);
                anim.SetTrigger("Activate");
                

                OnLeverHit();
            }
        }
    }

    public void OnLeverHit()
    {
        foreach(Gate gate in gates)
        {
            gate.TriggerGate();
        }
    }
}
