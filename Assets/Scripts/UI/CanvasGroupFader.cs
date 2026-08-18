using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGroupFader : MonoBehaviour
{
    public List<CanvasGroup> canvasGroupsToFadeIn;
    public List<CanvasGroup> canvasGroupsToFadeOut;
    public float fadeDuration = 1.0f;

    public void FadeIn()
    {
        CanvasGroupFaderController.instance.FadeIn(canvasGroupsToFadeIn, fadeDuration);
    }

    public void FadeOut()
    {
        CanvasGroupFaderController.instance.FadeOut(canvasGroupsToFadeOut, fadeDuration);
    }

    public void FadeOutThenFadeIn()
    {
        CanvasGroupFaderController.instance.FadeOutThenFadeIn(canvasGroupsToFadeOut, canvasGroupsToFadeIn, fadeDuration);
    }


}
