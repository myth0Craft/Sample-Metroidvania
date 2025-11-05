using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    [SerializeField] private Animator swordAnim;

    public void EnableSword()
    {
        swordAnim.SetBool("hasSword", true);
    }
}
