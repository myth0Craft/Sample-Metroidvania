using UnityEngine;
using System.IO;

public class SaveSystem
{
    private static SaveData saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData playerData;
    }


    public static string SaveFileName(int saveIndex)
    {

        saveIndex = Mathf.Clamp(saveIndex, 0, 3) + 1;

        string saveFile = Application.persistentDataPath + "/save" + saveIndex;
        return saveFile;
    }

    public static void Save(int saveIndex)
    {
        PlayerData.Save(ref saveData.playerData);

        File.WriteAllText(SaveFileName(saveIndex), JsonUtility.ToJson(saveData, true));
    }


    public static void Load(int saveIndex)
    {
        if (File.Exists(SaveFileName(saveIndex)))
        {
            string saveContent = File.ReadAllText(SaveFileName(saveIndex));
            saveData = JsonUtility.FromJson<SaveData>(saveContent);
            PlayerData.Load(saveData.playerData);
        } else
        {
            PlayerData.SetDefaults();
            PlayerData.Save(ref saveData.playerData);

            File.WriteAllText(SaveFileName(saveIndex), JsonUtility.ToJson(saveData, true));

        }
    }

}
