using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAsync : MonoBehaviour
{
    [SerializeField] private SceneAsset[] scenesToLoad;
    [SerializeField] private SceneAsset[] scenesToUnload;

    private GameObject player;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            LoadScenes();
            UnloadScenes();
        }

    }

    private void LoadScenes()
    {
        for (int i = 0; i < scenesToLoad.Length; i++)
        {
            bool isSceneLoaded = false;
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == scenesToLoad[i].name)
                {
                    isSceneLoaded = true;
                    break;
                }
            }

            if (!isSceneLoaded)
            {
                SceneManager.LoadSceneAsync(scenesToLoad[i].name, LoadSceneMode.Additive);
            }
        }


    }

    private void UnloadScenes()
    {
        for (int i = 0; i < scenesToUnload.Length; i++)
        {
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == scenesToLoad[i].name)
                {
                    SceneManager.UnloadSceneAsync(scenesToLoad[i].name);
                }
            }
        }
    }
}
