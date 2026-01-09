using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SavePoint : MonoBehaviour
{
    private Image saveIcon;
    private float saveIconFadeDuration = 0.5f;

    private void Awake()
    {
        saveIcon = GameObject.FindGameObjectWithTag("SaveIcon").GetComponent<Image>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SaveSystem.Save(PlayerData.saveIndex);
            StopCoroutine(DisplaySaveIcon());
            StartCoroutine(DisplaySaveIcon());
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
            StopCoroutine(DisplaySaveIcon());
            StartCoroutine(DisplaySaveIcon());
        }
    }

    public IEnumerator DisplaySaveIcon()
    {
        saveIcon.color = new Color(saveIcon.color.r, saveIcon.color.g, saveIcon.color.b, 0f);
        saveIcon.enabled = true;


        float elapsedPercentage = 0f;
        float elapsedTime = 0f;
        while (elapsedPercentage < 1)
        {
            elapsedPercentage = elapsedTime / saveIconFadeDuration;
            saveIcon.color = Color.Lerp(new Color(saveIcon.color.r, saveIcon.color.g, saveIcon.color.b, 0),
                new Color(saveIcon.color.r, saveIcon.color.g, saveIcon.color.b, 1), elapsedPercentage);
            yield return null;
            elapsedTime += Time.unscaledDeltaTime;
        }



        yield return new WaitForSecondsRealtime(1.0f);


        elapsedPercentage = 0f;
        elapsedTime = 0f;
        while (elapsedPercentage < 1)
        {
            elapsedPercentage = elapsedTime / saveIconFadeDuration;
            saveIcon.color = Color.Lerp(new Color(saveIcon.color.r, saveIcon.color.g, saveIcon.color.b, 1),
                new Color(saveIcon.color.r, saveIcon.color.g, saveIcon.color.b, 0), elapsedPercentage);
            yield return null;
            elapsedTime += Time.unscaledDeltaTime;
        }


        saveIcon.enabled = false;
    }
}