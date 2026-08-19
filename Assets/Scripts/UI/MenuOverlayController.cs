using UnityEngine;

public class MenuOverlayController : MonoBehaviour
{
    public static MenuOverlayController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void EnableMenuOverlay()
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }
    }

    public void DisableMenuOverlay()
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
    }
}
