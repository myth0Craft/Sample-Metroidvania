using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum PlayerColorScheme
{
    Default,
    Red,
    Green,
    Purple,
    White,
    Grey
}

[System.Serializable]
public struct ColorPalette
{
    public Color lightColor;
    public Color capeColor;
    public Color bodyColor;
}

[System.Serializable]
public struct ColorData
{
    public ColorPalette blue;
    public ColorPalette red;
    public ColorPalette green;
    public ColorPalette purple;
    public ColorPalette white;
    public ColorPalette grey;
}

public class PlayerCustomization : MonoBehaviour
{
    public Light2D spotlight;

    public Material capeMat;

    public Material bodyMat;

    public ColorData data;

    public void Awake()
    {
        SetPlayerColorScheme(PlayerData.colorScheme);
    }

    public void SetPlayerColorScheme(PlayerColorScheme colorScheme)
    {
        PlayerData.colorScheme = colorScheme;
        SaveSystem.Save(PlayerData.saveIndex);
        switch (colorScheme)
        {
            case PlayerColorScheme.Default:
                UpdateColors(data.blue.lightColor, data.blue.capeColor, data.blue.bodyColor);
                break;
            case PlayerColorScheme.Red:
                UpdateColors(data.red.lightColor, data.red.capeColor, data.red.bodyColor);
                break;
            case PlayerColorScheme.Green:
                UpdateColors(data.green.lightColor, data.green.capeColor, data.green.bodyColor);
                break;
            case PlayerColorScheme.Purple:
                UpdateColors(data.purple.lightColor, data.purple.capeColor, data.purple.bodyColor);
                break;
            case PlayerColorScheme.White:
                UpdateColors(data.white.lightColor, data.white.capeColor, data.white.bodyColor);
                break;
            case PlayerColorScheme.Grey:
                UpdateColors(data.grey.lightColor, data.grey.capeColor, data.grey.bodyColor);
                break;
        }
    }

    private void UpdateColors(Color lightColor, Color capeColor, Color bodyColor)
    {
        this.spotlight.color = lightColor;
        capeMat.SetColor("_Color", capeColor);
        bodyMat.SetColor("_Color", bodyColor);
    }
}
