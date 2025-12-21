using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public static class PlayerData
{
    private static PlayerControls globalControls = new PlayerControls();
    public static int maxHealth = 5;
    public static int currentHealth = 5;
    public static bool gamePaused = false;
    private static bool _allowGameInput = false;

    public static PlayerControls getControls()
    {
        return globalControls;
    }

    public static void AllowGameInput(bool allowGameInput)
    {
        _allowGameInput = allowGameInput;
        if (allowGameInput)
        {
            globalControls.Enable();
        } else
        {
            globalControls.Disable();
        }
    }
}
