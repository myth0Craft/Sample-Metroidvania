using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[System.Serializable]
public static class PlayerData
{
    public static int saveIndex = 0;
    private static PlayerControls globalControls = new PlayerControls();
    public static int maxHealth = 5;
    public static int currentHealth = 5;
    public static bool gamePaused = false;
    private static bool _allowGameInput = false;
    public static string currentScene = "Room1";
    public static Vector2 currentPosition = new Vector2(-3.0f, -1.0f);


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

    public static void Save(ref PlayerSaveData data)
    {
        data.position = currentPosition;
        data.maxHealth = maxHealth;
        data.currentScene = currentScene;
    }

    public static void Load(PlayerSaveData data)
    {
        currentPosition = data.position;
        maxHealth = data.maxHealth;
        currentScene = data.currentScene;
        currentHealth = data.maxHealth;
    }

    public static void SetDefaults()
    {
        currentPosition = new Vector2(-3.0f, -1.0f);
        currentScene = "Room1";
        maxHealth = 5;
        currentHealth = 5;
    }
}

[System.Serializable]
public struct PlayerSaveData
{
    public Vector2 position;
    public int maxHealth;
    public string currentScene;
}
