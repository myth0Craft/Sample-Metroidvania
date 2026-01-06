using UnityEngine;

public class InteractHintTrigger : MonoBehaviour
{
    private GameObject interactHintObj;
    public bool shouldCheckForCollision = true;

    private void Start()
    {
        interactHintObj = GameObject.FindGameObjectWithTag("InteractHintUI");
        interactHintObj = interactHintObj.transform.GetChild(0).gameObject;
        Debug.Log("interact hint trigger awake");
        if (interactHintObj == null)
        {
            Debug.Log("couldnt get the interact hint object");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && shouldCheckForCollision)
        {
            SetInteractPopupActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetInteractPopupActive(false);
        }
    }

    public void SetInteractPopupActive(bool active)
    {
        if (interactHintObj != null)
        {
            interactHintObj.SetActive(active);

        } else
        {
            Debug.Log("Interact hint UI obj not set");
        }
    }
}
