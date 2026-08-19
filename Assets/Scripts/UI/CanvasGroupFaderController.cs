using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CanvasGroupFaderController : MonoBehaviour
{
    public static CanvasGroupFaderController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }



    public void FadeIn(List<CanvasGroup> canvasGroupsToFadeIn, float duration)
    {
        StartCoroutine(FadeInCoroutine(canvasGroupsToFadeIn, duration));
    }

    public void FadeOut(List<CanvasGroup> canvasGroupsToFadeOut, float duration)
    {
        StartCoroutine(FadeOutCoroutine(canvasGroupsToFadeOut, duration));
    }

    public void FadeOutThenFadeIn(List<CanvasGroup> canvasGroupsToFadeOut, List<CanvasGroup> canvasGroupsToFadeIn, float duration)
    {
        StartCoroutine(FadeOutThenFadeInCoroutine(canvasGroupsToFadeOut, canvasGroupsToFadeIn, duration));
    }

    public IEnumerator FadeOutThenFadeInCoroutine(List<CanvasGroup> canvasGroupsToFadeOut, List<CanvasGroup> canvasGroupsToFadeIn, float duration)
    {

        yield return FadeOutCoroutine(canvasGroupsToFadeOut, duration);
        yield return FadeInCoroutine(canvasGroupsToFadeIn, duration);

    }

    public IEnumerator FadeInCoroutine(List<CanvasGroup> canvasGroupsToFadeIn, float duration)
    {
        foreach (CanvasGroup group in canvasGroupsToFadeIn)
        {
            group.gameObject.SetActive(true);
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }


        float startAlpha = 0f;
        float endAlpha = 1f;
        float elapsedTime = 0f;




        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            for (int i = 0; i < canvasGroupsToFadeIn.Count; i++)
            {
                canvasGroupsToFadeIn[i].alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            }

            yield return null;

        }

        foreach (CanvasGroup group in canvasGroupsToFadeIn)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
    }

    public IEnumerator FadeOutCoroutine(List<CanvasGroup> canvasGroupsToFadeOut, float duration)
    {

        foreach (CanvasGroup group in canvasGroupsToFadeOut)
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }


        float startAlpha = 1f;
        float endAlpha = 0f;
        float elapsedTime = 0f;




        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            for (int i = 0; i < canvasGroupsToFadeOut.Count; i++)
            {
                canvasGroupsToFadeOut[i].alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            }

            yield return null;

        }

        foreach (CanvasGroup group in canvasGroupsToFadeOut)
        {
            group.alpha = 0f;
            group.gameObject.SetActive(false);
        }
    }
}
