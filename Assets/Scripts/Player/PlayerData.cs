using UnityEngine;

[System.Serializable]
public static class PlayerData
{
    private static PlayerControls globalControls = new PlayerControls();
    public static int maxHealth = 5;
    public static int currentHealth = 5;

    public static PlayerControls getControls()
    {
        return globalControls;
    }
}
