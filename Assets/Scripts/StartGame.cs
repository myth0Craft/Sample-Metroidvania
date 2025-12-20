using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    private string persistentGame = "PersistentData";
    public void LoadGame()
    {
        SceneManager.UnloadSceneAsync("Title");
        SceneManager.LoadSceneAsync(persistentGame);
    }
}
