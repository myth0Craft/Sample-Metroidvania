using UnityEngine;

public class PlayerPause : MonoBehaviour
{
    private PlayerControls controls;
    private bool pausePressed;

    private bool gamePaused = false;

    [SerializeField] private PauseScreen pauseScreen;

    private void Awake()
    {
        controls = PlayerData.getControls();
        controls.Player.Pause.performed += ctx => pausePressed = true;
    }

    private void Update()
    {
        if (pausePressed && !gamePaused)
        {
            pausePressed = false;
            gamePaused = true;
            Time.timeScale = 0;
            if (pauseScreen != null)
            {
                pauseScreen.OnGamePause();
            }
        }
        else if (pausePressed && gamePaused)
        {
            pausePressed = false;
            gamePaused = false;
            Time.timeScale = 1;
            if (pauseScreen != null)
            {
                pauseScreen.OnGameUnpause();
            }
        }
    }

    public bool IsPaused()
    {
        return gamePaused;
    }
}
