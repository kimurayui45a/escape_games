using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
    [SerializeField]
    Image fadeImage;
    [SerializeField]
    GameObject nowLoading;

    public bool IsFadeIn { get; set; } = false;
    Coroutine fadeCoroutine;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void PlayFadeIn(float duration, System.Action isFadeEnd)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        fadeCoroutine = StartCoroutine(FadeCoroutine(0, 1, duration, isFadeEnd));
    }
    public void PlayFadeOut(float duration, System.Action isFadeEnd)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        fadeCoroutine = StartCoroutine(FadeCoroutine(1, 0, duration, isFadeEnd));
    }

    IEnumerator FadeCoroutine(float startAlpha, float endAlpha, float duration, System.Action isFadeEnd)
    {
        float time = 0;

        nowLoading.SetActive(false);
        while (time < duration)
        {
            time += Time.deltaTime;

            var alpha = startAlpha + (endAlpha - startAlpha) * time / duration;

            var tempColor = fadeImage.color;
            tempColor.a = alpha;
            fadeImage.color = tempColor;
            yield return null;
        }
        {
            var tempColor = fadeImage.color;
            tempColor.a = endAlpha;
            fadeImage.color = tempColor;
        }
        fadeCoroutine = null;
        IsFadeIn = endAlpha > 0;
        if (IsFadeIn)
        {
            nowLoading.SetActive(true);
        }
        isFadeEnd?.Invoke();
    }
}
