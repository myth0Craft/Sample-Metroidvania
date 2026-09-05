using TMPro;
using UnityEngine;
using System;

public class BetaFeaturesToggleButton : MonoBehaviour
{
    private TextMeshProUGUI text;

    public static event Action onBetaFeaturesToggled;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();

        UpdateButtonText();
    }

    public void ToggleBetaFeatures()
    {
        PlayerData.betaFeaturesEnabled = !PlayerData.betaFeaturesEnabled;
        UpdateButtonText();

        if (onBetaFeaturesToggled != null)
        {
            onBetaFeaturesToggled.Invoke();
        }
    }

    private void UpdateButtonText()
    {
        if (PlayerData.betaFeaturesEnabled)
        {
            text.text = "Beta Features: Enabled";
        }
        else
        {
            text.text = "Beta Features: Disabled";
        }
    }
}