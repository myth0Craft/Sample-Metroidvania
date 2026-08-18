using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CanvasGroupFaderController : MonoBehaviour
{
    public static CanvasGroupFaderController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
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
        for (int i = 0; i < canvasGroupsToFadeIn.Count; i++)
        {
            canvasGroupsToFadeIn[i].gameObject.SetActive(true);
            canvasGroupsToFadeIn[i].alpha = 0f;
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

        for (int i = 0; i < canvasGroupsToFadeIn.Count; i++)
        {
            canvasGroupsToFadeIn[i].alpha = 1f;
        }
    }

    public IEnumerator FadeOutCoroutine(List<CanvasGroup> canvasGroupsToFadeOut, float duration)
    {

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

        for (int i = 0; i < canvasGroupsToFadeOut.Count; i++)
        {
            canvasGroupsToFadeOut[i].gameObject.SetActive(false);
            canvasGroupsToFadeOut[i].alpha = 0f;
        }
    }
}
