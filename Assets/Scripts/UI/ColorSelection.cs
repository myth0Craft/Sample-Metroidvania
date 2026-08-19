using UnityEngine;
using UnityEngine.UI;

public class ColorSelection : MonoBehaviour
{
    public Image image;
    public PlayerColorScheme colorScheme;

    private void Awake()
    {
        if (PlayerData.colorScheme == colorScheme)
        {
            image.gameObject.SetActive(true);
        } else
        {
            image.gameObject.SetActive(false);
        }
    }

    
}