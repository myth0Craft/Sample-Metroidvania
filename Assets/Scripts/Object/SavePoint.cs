using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : MonoBehaviour
{


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SaveSystem.Save(PlayerData.saveIndex);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerData.currentScene = gameObject.scene.name;
            PlayerData.posX = gameObject.transform.position.x;
            PlayerData.posY = gameObject.transform.position.y;
            SaveSystem.Save(PlayerData.saveIndex);
        }
    }
}