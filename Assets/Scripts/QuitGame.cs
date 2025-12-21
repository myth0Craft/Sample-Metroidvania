using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitGame : MonoBehaviour
{
    public void LoadTitleScreen()
    {
        Time.timeScale = 1;
        PlayerData.gamePaused = false;
        SceneManager.LoadScene("Title");
    }
}
