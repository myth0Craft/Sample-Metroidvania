using UnityEngine;

public class PlayerPause : MonoBehaviour
{
    private PlayerControls controls;
    private bool pausePressed;

    private bool gamePaused = false;

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
        }
        else if (pausePressed && gamePaused)
        {
            pausePressed = false;
            gamePaused = false;
            Time.timeScale = 1;
        }
    }

    public bool IsPaused()
    {
        return gamePaused;
    }
}
