using UnityEngine;
using UnityEngine.UI;

public class ColorSelectionButton : MonoBehaviour
{
    [SerializeField] private PlayerColorScheme colorScheme;
    private Button button;

    public void SelectColor()
    {
        PlayerCustomizationController.instance.SetPlayerColorScheme(colorScheme);
    }

    private void OnEnable()
    {
        if (PlayerData.colorScheme == this.colorScheme)
        {
            button.Select();
        }
    }

    private void Awake()
    {
        button = GetComponent<Button>();
    }
}
