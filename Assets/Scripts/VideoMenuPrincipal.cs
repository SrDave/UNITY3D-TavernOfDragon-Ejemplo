using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class VideoMenuPrincipal : MonoBehaviour
{
    public VideoPlayer videoPlayer;       // Arrastra aquí el VideoPlayer
    public VideoClip[] videoClips;        // Arrastra aquí los videos en el inspector
    private int currentVideoIndex = 0;    // Índice del video actual
    private bool isKeyDetectionActive = true; // Controla si la detección de teclas está activa
    private bool isFirstVideo = true;     // Indica si el primer video está en reproducción

    [Header("Skip Text Config")]
    public TextMeshProUGUI skipText;      // Texto "Presiona cualquier tecla para omitir presentación"
    public float fadeDuration = 1f;      // Duración del efecto fade in/out
    public float fadeCycleDelay = 1f;    // Tiempo entre ciclos de fade in/out

    [Header("UI Elements")]
    public List<GameObject> textElements; // Lista de textos a desactivar
    public GameObject panel;             // Panel que se mantendrá activo
    public UIManager uiManager; // Referencia al UIManager


    void Start()
    {
        // Asegurar que el texto comienza invisible
        if (skipText != null)
        {
            SetAlpha(skipText, 0f);
            skipText.gameObject.SetActive(false);
        }

        // Reproducir el primer video
        PlayVideo(currentVideoIndex);

        // Suscribir correctamente el evento
        videoPlayer.loopPointReached += OnVideoEnd;

        // Iniciar el ciclo de fade del texto si estamos en el primer video
        if (skipText != null && isFirstVideo)
        {
            skipText.gameObject.SetActive(true);
            StartCoroutine(HandleSkipTextFade());
        }
    }

    void Update()
    {
        // Si estamos en el primer video y se presiona cualquier tecla
        if (currentVideoIndex == 0 && isKeyDetectionActive && Input.anyKeyDown)
        {
            SkipToNextVideo();
        }
    }

    void PlayVideo(int index)
    {
        if (videoClips.Length == 0) return; // Si no hay videos, no hacer nada

        videoPlayer.Stop(); // Detener el video actual antes de cambiar
        videoPlayer.clip = videoClips[index];

        // Si es el último video, activamos el bucle
        videoPlayer.isLooping = (index == videoClips.Length - 1);

        videoPlayer.Play();
    }

    void SkipToNextVideo()
    {
        // Saltar al segundo video directamente
        currentVideoIndex = Mathf.Min(currentVideoIndex + 1, videoClips.Length - 1);
        isKeyDetectionActive = false; // Desactivar la detección de teclas después del salto
        isFirstVideo = false;         // El primer video ya no está en reproducción

        // Desactivar todos los textos excepto los del panel usando el UIManager
        if (uiManager != null)
        {
            uiManager.DeactivateTextsExceptPanel(panel);

            // Desactivar las corutinas en UIManager
            uiManager.isSkipActive = true; // Detener las corutinas de fade

            // Forzar la activación del panel de inmediato
            uiManager.ForceActivatePanel(panel);
        }

        // Desaparecer el texto con fade
        if (skipText != null)
        {
            StopCoroutine(HandleSkipTextFade());
            StartCoroutine(FadeText(skipText, skipText.color.a, 0f, fadeDuration));
        }

        PlayVideo(currentVideoIndex);
    }



    void OnVideoEnd(VideoPlayer vp)
{
        if (currentVideoIndex == 0)
        {
            // Si termina el primer video, asegurar que el texto desaparezca
            isFirstVideo = false;
            if (skipText != null)
            {
                StopCoroutine(HandleSkipTextFade());
                StartCoroutine(FadeText(skipText, skipText.color.a, 0f, fadeDuration));
            }
        }

        // Restablecer la variable isSkipActive al final del primer video
        if (currentVideoIndex > 0)
        {
            if (uiManager != null)
            {
                uiManager.isSkipActive = false; // Permitir que las corutinas sigan funcionando
            }
        }

        // Avanzar al siguiente video
        if (currentVideoIndex < videoClips.Length - 1)
        {
            currentVideoIndex++;
            PlayVideo(currentVideoIndex);
        }
    }

    private IEnumerator HandleSkipTextFade()
    {
        while (isFirstVideo)
        {
            // Fade in
            yield return StartCoroutine(FadeText(skipText, 0f, 1f, fadeDuration));

            // Mantener visible
            yield return new WaitForSeconds(fadeCycleDelay);

            // Fade out
            yield return StartCoroutine(FadeText(skipText, 1f, 0f, fadeDuration));

            // Mantener invisible
            yield return new WaitForSeconds(fadeCycleDelay);
        }
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            SetAlpha(text, alpha);
            yield return null;
        }
        SetAlpha(text, endAlpha);
    }

    private void SetAlpha(TextMeshProUGUI text, float alpha)
    {
        if (text != null)
        {
            var color = text.color;
            color.a = alpha;
            text.color = color;
        }
    }

    private void OnDestroy()
    {
        // Desuscribir el evento para evitar problemas
        videoPlayer.loopPointReached -= OnVideoEnd;
    }
}
