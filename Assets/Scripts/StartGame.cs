using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public string sceneName;
    public string persistentGame;
    public void LoadGame()
    {
        SceneManager.LoadScene(persistentGame);
        SceneManager.LoadScene(sceneName);
        SceneManager.UnloadSceneAsync("Title");
        print("game started");
    }
}
