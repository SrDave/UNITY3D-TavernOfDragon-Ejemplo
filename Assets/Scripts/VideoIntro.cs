using UnityEngine;
using UnityEngine.SceneManagement; // Para gestionar escenas
using UnityEngine.Video; // Para manejar videos

public class VideoIntro : MonoBehaviour
{
    public VideoPlayer videoIntro; // Asigna tu VideoPlayer en el Inspector
    private bool isSkipping = false; // Evita que la escena se cargue múltiples veces

    // Start is called before the first frame update
    private void Start()
    {
        if (videoIntro != null)
        {
            GameManager.instance?.SetSavingEnabled(false); // Desactiva el guardado
            videoIntro.Play();
            videoIntro.loopPointReached += OnVideoEnd;
        }
        else
        {
            Debug.LogError("No se ha asignado el VideoPlayer.");
        }
    }

    // Método que se llama cuando el video termina
    private void OnVideoEnd(VideoPlayer vp)
    {
        if (!isSkipping) // Evita múltiples llamadas
        {
            isSkipping = true;
            Debug.Log("Video finalizado. Cargando escena...");
            GameManager.instance?.SetSavingEnabled(true); // Reactiva el guardado
            LoadNextScene();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Si se presiona cualquier tecla o botón del ratón
        if (Input.anyKeyDown && !isSkipping)
        {
            Debug.Log("Salteando el video. Cargando escena...");
            isSkipping = true;
            if (videoIntro != null && videoIntro.isPlaying)
            {
                videoIntro.Stop(); // Detiene el video
            }
            GameManager.instance?.SetSavingEnabled(true); // Reactiva el guardado
            LoadNextScene();
        }
    }

    // Carga la siguiente escena
    private void LoadNextScene()
    {
        SceneManager.LoadScene("La taberna del Dragon"); // Cambia el nombre por el de tu escena
    }

    private void OnDestroy()
    {
        if (videoIntro != null)
        {
            videoIntro.loopPointReached -= OnVideoEnd; // Desuscribirse del evento para evitar errores
        }
    }
}
