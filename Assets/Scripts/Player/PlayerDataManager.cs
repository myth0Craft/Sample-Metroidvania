using UnityEngine;

public class PlayerDataManager
{
    public static PlayerDataManager Instance { get; private set; } = new PlayerDataManager();
    public PlayerData Data { get; private set; } = new PlayerData();


}
