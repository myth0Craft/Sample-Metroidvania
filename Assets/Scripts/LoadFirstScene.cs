using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirstScene : MonoBehaviour
{

    [SerializeField] private string sceneToLoad;
    void Start()
    {
        if (!SceneManager.GetSceneByName(sceneToLoad).isLoaded)
            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
    }
}
