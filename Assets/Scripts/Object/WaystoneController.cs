using UnityEngine;

public class WaystoneController : MonoBehaviour
{
    [SerializeField] private GameObject inactive;
    [SerializeField] private GameObject active;

    [SerializeField] private bool waystoneActive = true;

    private InteractHintTrigger interactHintTrigger;

    private bool interactPressed;
    private PlayerControls controls;

    private void Awake()
    {
        controls = PlayerData.getControls();
        interactHintTrigger = GetComponent<InteractHintTrigger>();
        controls.Player.Interact.performed += ctx => interactPressed = true;
        if (waystoneActive)
        {
            inactive.SetActive(false);
            active.SetActive(true);
        } else
        {
            inactive.SetActive(true);
            active.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        //interactHintTrigger.SetInteractPopupActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && waystoneActive)
        {
            if (interactPressed)
            {
                Deactivate();
            }
        }
    }

    private void Deactivate()
    {
        interactHintTrigger.SetInteractPopupActive(false);
        interactHintTrigger.shouldCheckForCollision = false;
        inactive.SetActive(true);
        active.SetActive(false);
    }
}
