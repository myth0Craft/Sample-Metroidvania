using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TutorialUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup moveTutorial;
    [SerializeField] private CanvasGroup jumpTutorial;
    [SerializeField] private CanvasGroup swordTutorial;
    [SerializeField] private CanvasGroup upAttackTutorial;
    [SerializeField] private CanvasGroup downAttackTutorial;
    [SerializeField] private CanvasGroup staminaTutorial;
    [SerializeField] private CanvasGroup parryTutorial;

    private CanvasGroupFader fader;

    public float duration = 0.5f;

    [SerializeField] private Volume volumeOverlay;
    private Vignette vignette;

    public static TutorialUIController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
        fader = GetComponent<CanvasGroupFader>();
    }

    private IEnumerator FadeInVolumeOverlay(Vector2 vignetteCenter)
    {
        //player screen space pos
        Vector3 screenPos = Camera.main.WorldToScreenPoint(PlayerMovement.instance.gameObject.transform.position);

        volumeOverlay.gameObject.SetActive(true);

        if (volumeOverlay.profile.TryGet(out vignette))
        {
            vignette.center.overrideState = true;
            vignette.center.value = vignetteCenter;

            vignette.intensity.overrideState = true;
            vignette.intensity.value = 0f;

            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                vignette.intensity.value = (float)((elapsedTime * 0.4) / duration);



                elapsedTime += Time.deltaTime;
                yield return null;
            }

            vignette.intensity.value = 0.4f;
        }
    }

    private IEnumerator FadeOutVolumeOverlay()
    {
        if (volumeOverlay.profile.TryGet(out vignette))
        {

            vignette.intensity.overrideState = true;
            vignette.intensity.value = 4f;

            float elapsedTime = duration;

            while (elapsedTime > 0)
            {
                vignette.intensity.value = (float)((elapsedTime * 0.4) / duration);

                elapsedTime -= Time.deltaTime;
                yield return null;
            }

            vignette.intensity.value = 0.0f;
        }

        volumeOverlay.gameObject.SetActive(false);
    }


    private void FadeInTutorial(CanvasGroup groupToFadeIn)
    {
        fader.canvasGroupsToFadeIn = new System.Collections.Generic.List<CanvasGroup> { groupToFadeIn };
        fader.fadeDuration = duration;
        fader.FadeIn();
    }

    private void FadeOutTutorial(CanvasGroup groupToFadeOut)
    {
        fader.canvasGroupsToFadeOut = new System.Collections.Generic.List<CanvasGroup> { groupToFadeOut };
        fader.fadeDuration = duration;
        fader.FadeOut();
    }

    public void PlayWalkTutorial()
    {
        FadeInTutorial(moveTutorial);
        StartCoroutine(FadeInVolumeOverlay(Camera.main.WorldToScreenPoint(PlayerMovement.instance.gameObject.transform.position)));
    }

    public void PlayJumpTutorial()
    {
        FadeInTutorial(jumpTutorial);
        StartCoroutine(FadeInVolumeOverlay(Camera.main.WorldToScreenPoint(PlayerMovement.instance.gameObject.transform.position)));
    }

    public void PlaySwordTutorial()
    {
        FadeInTutorial(swordTutorial);
        StartCoroutine(FadeInVolumeOverlay(Camera.main.WorldToScreenPoint(PlayerMovement.instance.gameObject.transform.position)));
    }

    public void PlayUpAttackTutorial()
    {
        FadeInTutorial(upAttackTutorial);
        StartCoroutine(FadeInVolumeOverlay(Camera.main.WorldToScreenPoint(PlayerMovement.instance.gameObject.transform.position)));
    }

    public void PlayDownAttackTutorial()
    {
        FadeInTutorial(downAttackTutorial);
        StartCoroutine(FadeInVolumeOverlay(Camera.main.WorldToScreenPoint(PlayerMovement.instance.gameObject.transform.position)));
    }

    public void PlayStaminaTutorial()
    {
        FadeInTutorial(staminaTutorial);
        StartCoroutine(FadeInVolumeOverlay(new Vector2(0.85f, 0.8f)));
    }

    public void PlayParryTutorial()
    {
        FadeInTutorial(parryTutorial);
        StartCoroutine(FadeInVolumeOverlay(new Vector2(0.85f, 0.8f)));
    }
}