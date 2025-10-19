using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    private PlayerControls controls;
    private PlayerMovement playerMovement;
    private bool attackPressed;
    private float attackDurationSeconds = 0.2f;
    private float attackTimer = 0;
    BoxCollider2D playerBox;
    [SerializeField] private BoxCollider2D attackHitbox;

    private void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerBox = GetComponentInParent<BoxCollider2D>();
        controls = new PlayerControls();
        controls.Player.Attack.performed += ctx => attackPressed = true;
    }

    private void Update()
    {
        if (attackPressed)
        {
            StartAttack();
            attackPressed = false;
        }
    }



    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    public void StartAttack()
    {


        attackHitbox.offset = playerMovement.getFacingDirection() ? new Vector2(1, 0) : new Vector2(-1, 0);
    }
}
