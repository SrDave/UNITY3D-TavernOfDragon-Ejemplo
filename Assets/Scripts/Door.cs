using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Importa TextMeshPro

public class Door : MonoBehaviour
{
    public Animator anim; // Control de animaciones
    public AudioClip DoorOpenSound;
    public AudioClip DoorCloseSound;
    private AudioSource audiosource;

    public float activationDistance = 2f; // Distancia máxima para interactuar
    public Transform player; // Referencia al jugador o la cámara

    public TextMeshProUGUI interactionText; // Texto TMP en pantalla para "Press E button"
    private int activationCount = 0;
    private Renderer doorRenderer; // Para cambiar el material o resaltar
    private Color originalColor; // Guardar el color original de la puerta
    private bool isPlayerInRange = false; // Indica si el jugador está dentro del rango
    private Coroutine fadeCoroutine; // Para manejar la corutina del fade-in y fade-out

    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        doorRenderer = GetComponent<Renderer>();
        originalColor = doorRenderer.material.color; // Guarda el color inicial

        // Asegurarnos de que el texto esté invisible al inicio
        if (interactionText != null)
        {
            SetTextAlpha(0f); // Configura la transparencia del texto a 0
            interactionText.enabled = false;
        }
    }

    void Update()
    {
        // Comprobar si el jugador está dentro del rango
        float distance = Vector3.Distance(player.position, transform.position);
        bool wasPlayerInRange = isPlayerInRange; // Guarda el estado anterior
        isPlayerInRange = distance <= activationDistance;

        // Si el estado de rango ha cambiado, iniciar el efecto de transparencia
        if (isPlayerInRange != wasPlayerInRange)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine); // Detener cualquier efecto en curso
            }

            if (isPlayerInRange)
            {
                interactionText.enabled = true; // Asegura que el texto esté activo
                fadeCoroutine = StartCoroutine(FadeTextToAlpha(1f)); // Aparecer
            }
            else
            {
                fadeCoroutine = StartCoroutine(FadeTextToAlpha(0f)); // Desaparecer
            }
        }
        // Cambiar el color de la puerta según el rango
        HighlightDoor(isPlayerInRange);

        // Detectar si el jugador presiona "Q" y está en rango
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Q))
        {
            ToggleDoor();
        }
    }

    private void ToggleDoor()
    {
        activationCount++;

        if (activationCount % 2 == 1) // Si el contador es impar, abrir la puerta
        {
            anim.SetTrigger("DoorOpen");
           
            audiosource.PlayOneShot(DoorOpenSound);
            
        }
        else // Si el contador es par, cerrar la puerta
        {
            anim.SetTrigger("DoorClose");
           
            audiosource.PlayOneShot(DoorCloseSound);
            
        }
    }

    private IEnumerator FadeTextToAlpha(float targetAlpha)
    {
        float currentAlpha = interactionText.color.a;
        float duration = 1.5f; // Duración de la transición
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, elapsedTime / duration);
            SetTextAlpha(newAlpha);
            yield return null;
        }

        SetTextAlpha(targetAlpha); // Asegurarse de que alcanza el valor final

        // Si se ha desvanecido completamente, desactiva el texto
        if (targetAlpha == 0f)
        {
            interactionText.enabled = false;
        }
    }

    private void SetTextAlpha(float alpha)
    {
        if (interactionText != null)
        {
            Color color = interactionText.color;
            color.a = alpha; // Ajusta el canal alfa
            interactionText.color = color;
        }
    }

    private void HighlightDoor(bool highlight)
    {
        if (doorRenderer != null)
        {
            if (highlight)
            {
                doorRenderer.material.color = Color.blue; // Cambia a un color resaltado
            }
            else
            {
                doorRenderer.material.color = originalColor; // Restaura el color original
            }
        }
    }
}
