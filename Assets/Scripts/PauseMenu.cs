using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using System.Collections;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel; // El panel que contiene el menú de pausa
    private bool isPaused = false; // Estado del juego (en pausa o no)
    public Image fadePanel; // Panel negro para el efecto de fade
    public TextMeshProUGUI chapterText;
    public float panelFade = 2f;
    public float textFadeDuration = 3f;

    void Start()
    {
        fadePanel.gameObject.SetActive(true);
        chapterText.gameObject.SetActive(false);
        StartCoroutine(FadeIn());
        StartCoroutine(FadeInOutText());
        
    }

    void Update()
    {
        // Detectar la tecla ESC para alternar el menú de pausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Cursor.visible = true; // Mostrar el cursor
        Cursor.lockState = CursorLockMode.None; // Liberar el cursor
        pausePanel.SetActive(true); // Mostrar el panel de pausa
        //Time.timeScale = 0f; // Detener el tiempo del juego
        Debug.Log("Juego Pausado");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Cursor.visible = false; // Mostrar el cursor
        Cursor.lockState = CursorLockMode.Locked; // Bloquear el cursor
        pausePanel.SetActive(false); // Ocultar el panel de pausa
        //Time.timeScale = 1f; // Reanudar el tiempo del juego
        Debug.Log("Juego Activo");
    }

    public void VolverMenu()
    {
        Time.timeScale = 1f; // Asegúrate de reanudar el tiempo por si se pausó
        SceneManager.LoadScene("MenuPrincipal"); // Carga la escena del menú principal
        Debug.Log("Vuelta a Menu");
    }

    public void SalirJuego()
    {
        Time.timeScale = 1f; // Asegurarse de que el tiempo esté reanudado
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Salir del modo Play en el editor
    #else
        Application.Quit(); // Salir de la aplicación en la build
    #endif
    }

    public void SalvarPartida()
    {
        if (GameManager.instance != null)
        {
            Debug.Log("GameManager encontrado, guardando...");
            GameManager.instance.SaveGame();
            Debug.Log("Partida Salvada");
        }
        else
        {
            Debug.LogError("No se puede guardar el juego: GameManager no encontrado.");
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color panelColor = fadePanel.color;
        panelColor.a = 1f; // Asegurarse de que comienza completamente opaco

        while (elapsedTime < panelFade)
        {
            elapsedTime += Time.deltaTime;
            panelColor.a = Mathf.Lerp(1f, 0f, elapsedTime / panelFade); // Interpolación del alfa
            fadePanel.color = panelColor; // Aplicar el nuevo color
            yield return null;
        }

        panelColor.a = 0f; // Asegurarse de que termina completamente transparente
        fadePanel.color = panelColor;
    }

    private IEnumerator FadeInOutText()
    {
        // Fade In
        chapterText.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Color textColor = chapterText.color;
        textColor.a = 0f; // Comienza transparente

        while (elapsedTime < textFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Lerp(0f, 1f, elapsedTime / textFadeDuration);
            chapterText.color = textColor;
            yield return null;
        }

        textColor.a = 1f; // Asegurarse de que termina completamente visible
        chapterText.color = textColor;

        // Esperar un tiempo antes de iniciar el fade out
        yield return new WaitForSeconds(2f); // Ajusta el tiempo de espera según lo necesites

        // Fade Out
        elapsedTime = 0f;
        while (elapsedTime < textFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Lerp(1f, 0f, elapsedTime / textFadeDuration);
            chapterText.color = textColor;
            yield return null;
        }

        textColor.a = 0f; // Asegurarse de que termina completamente transparente
        chapterText.color = textColor;
    }


}
