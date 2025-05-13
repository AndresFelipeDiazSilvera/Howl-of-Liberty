using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class FadeController : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;

    void Start()
    {
        // Puedes iniciar con fade-in si quieres
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    public void StartFadeOutThenIn(Action onMidFade)
    {
        StartCoroutine(FadeSequence(onMidFade));
    }

    private IEnumerator FadeSequence(Action onMidFade)
    {
        yield return StartCoroutine(FadeOut());

        // Ejecutar la acción entre el fade out y fade in
        onMidFade?.Invoke();

        yield return new WaitForSeconds(0.5f); // Pequeña pausa opcional
        yield return StartCoroutine(FadeIn());
    }
}
