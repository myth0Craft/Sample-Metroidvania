using UnityEngine;

public class InteractHintTrigger : MonoBehaviour
{
    private GameObject interactHintObj;
    public bool shouldCheckForCollision = true;

    private void Awake()
    {
        interactHintObj = GameObject.FindGameObjectWithTag("InteractHintUI");
        interactHintObj = interactHintObj.transform.GetChild(0).gameObject;
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
        interactHintObj.SetActive(active);
    }
}
