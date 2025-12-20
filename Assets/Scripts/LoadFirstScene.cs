using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirstScene : MonoBehaviour
{
    void Start()
    {
        if (!SceneManager.GetSceneByName("Room1").isLoaded)
            SceneManager.LoadSceneAsync("Room1", LoadSceneMode.Additive);
    }
}
