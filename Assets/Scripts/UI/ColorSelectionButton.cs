using UnityEngine;
using UnityEngine.UI;
using System;

public class ColorSelectionButton : MonoBehaviour
{
    [SerializeField] private PlayerColorScheme colorScheme;
    private Button button;
    [SerializeField ]private Image selectionImage;
    public static event Action<ColorSelectionButton> OnColorSelected;

    public void SelectColor()
    {
        OnColorSelected?.Invoke(this);
    }

    private void Awake()
    {
        button = GetComponent<Button>();
        if (PlayerData.colorScheme == this.colorScheme)
        {
            button.Select();
            selectionImage.gameObject.SetActive(true);
        } else
        {
            selectionImage.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        OnColorSelected += HandleColorSelected;
    }

    private void OnDisable()
    {
        OnColorSelected -= HandleColorSelected;
    }

    private void HandleColorSelected(ColorSelectionButton button)
    {
        if (button == this)
        {
            PlayerData.colorScheme = this.colorScheme;
            PlayerCustomizationController.instance.SetPlayerColorScheme(colorScheme);
            selectionImage.gameObject.SetActive(true);
        } else
        {
            selectionImage.gameObject.SetActive(false);
        }
    }
}
