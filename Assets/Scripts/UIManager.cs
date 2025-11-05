using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Para el tipo Graphic
using UnityEngine.SceneManagement; // Para gestionar escenas
using TMPro;
using UnityEngine.Video; // Para manejar videos

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public class UIConfig
    {
        public Graphic uiElement;    // Referencia al elemento UI (texto, panel, etc.)
        public float startTime;      // Momento en segundos en el que el elemento debe aparecer
        public float fadeDuration;   // Duración del fade in/out
        public float durationVisible; // Duración que el elemento permanece visible
    }
    public bool isSkipActive = false; // Controla si las corutinas deben continuar
    public GameObject panelSetting; // Arrastra el PanelSetting desde el Inspector
    public GameObject panelBT;
    public List<UIConfig> uiElements; // Lista de configuraciones de UI

    public GameObject continueButton;

    public TMP_InputField inputUp;
    public TMP_InputField inputDown;
    public TMP_InputField inputLeft;
    public TMP_InputField inputRight;
    public TMP_InputField inputJump;
    public TMP_InputField inputDodge;
    public TMP_InputField inputSpell;
    public TMP_InputField inputInteract;


    private void Start()
    {
        
        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.Save();
        ConfigureContinueButton();
        LoadKeyBindings();
        foreach (var uiConfig in uiElements)
        {
            // Asegurar que el objeto está inicialmente invisible
            InitializeVisibility(uiConfig.uiElement);

            // Iniciar la secuencia de aparición y desaparición
            StartCoroutine(HandleUIFade(uiConfig));
        }
    }

    private void ConfigureContinueButton()
    {       
        // Verificar si el jugador ya ha iniciado una partida
        if (PlayerPrefs.GetInt("HasPlayedBefore", 0) == 1)
        {
            continueButton.SetActive(true); // Mostrar botón Continuar
        }
        else
        {
            continueButton.SetActive(false); // Ocultar botón Continuar
        }
    }
    

    private void InitializeVisibility(Graphic uiElement)
    {
        if (uiElement != null)
        {
            // Desactiva el objeto al inicio
            uiElement.gameObject.SetActive(false);
        }
    }

    private IEnumerator HandleUIFade(UIConfig config)
    {
        // Esperar hasta el tiempo de inicio solo si el skip no está activo
        yield return new WaitForSeconds(config.startTime);

        // Verificar si la detección de skip está activa antes de proceder
        if (isSkipActive)
            yield break; // Salir si la acción de skip se ha activado

        // Activar el elemento antes de iniciar el fade
        config.uiElement.gameObject.SetActive(true);

        // Fade in (aparecer)
        yield return StartCoroutine(FadeUI(config.uiElement, 0f, 1f, config.fadeDuration));

        // Mantener el elemento visible durante el tiempo especificado
        yield return new WaitForSeconds(config.durationVisible);

        // Fade out (desaparecer)
        yield return StartCoroutine(FadeUI(config.uiElement, 1f, 0f, config.fadeDuration));

        // Desactivar el objeto después del fade out
        config.uiElement.gameObject.SetActive(false);
    }

    private IEnumerator FadeUI(Graphic uiElement, float startAlphaFactor, float endAlphaFactor, float duration)
    {
        if (uiElement == null) yield break;

        // Obtenemos el alfa original del elemento
        var originalAlpha = uiElement.color.a;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Calculamos el nuevo alfa en función del factor
            float alpha = Mathf.Lerp(originalAlpha * startAlphaFactor, originalAlpha * endAlphaFactor, elapsedTime / duration);

            // Aplicamos el alfa
            SetAlpha(uiElement, alpha);

            yield return null;
        }

        // Asegurar que el alpha final se establece correctamente
        SetAlpha(uiElement, originalAlpha * endAlphaFactor);
    }

    private void SetAlpha(Graphic uiElement, float alpha)
    {
        if (uiElement != null)
        {
            var color = uiElement.color;
            color.a = alpha; // Modificar el canal alfa
            uiElement.color = color;
        }
    }

    public void SaveKeyBindings()
    {
        PlayerPrefs.SetString("KeyUp", inputUp.text);
        PlayerPrefs.SetString("KeyDown", inputDown.text);
        PlayerPrefs.SetString("KeyLeft", inputLeft.text);
        PlayerPrefs.SetString("KeyRight", inputRight.text);
        PlayerPrefs.SetString("KeyJump", inputJump.text);
        PlayerPrefs.SetString("KeyDodge", inputDodge.text);
        PlayerPrefs.SetString("KeySpell", inputSpell.text);
        PlayerPrefs.SetString("KeyInteract", inputInteract.text);

        PlayerPrefs.Save();
        Debug.Log("Key bindings saved!");
    }

    private void LoadKeyBindings()
    {
        inputUp.text = PlayerPrefs.GetString("KeyUp", "W");
        inputDown.text = PlayerPrefs.GetString("KeyDown", "S");
        inputLeft.text = PlayerPrefs.GetString("KeyLeft", "A");
        inputRight.text = PlayerPrefs.GetString("KeyRight", "D");
        inputJump.text = PlayerPrefs.GetString("KeyJump", "Space");
        inputDodge.text = PlayerPrefs.GetString("KeyDodge", "LeftShift");
        inputSpell.text = PlayerPrefs.GetString("KeySpell", "E");
        inputInteract.text = PlayerPrefs.GetString("KeyInteract", "Q");
    }

    public void DeactivateTextsExceptPanel(GameObject panel)
    {
        foreach (var uiConfig in uiElements)
        {
            if (uiConfig.uiElement != null)
            {
                if (uiConfig.uiElement.transform.parent != panel.transform)
                {
                    uiConfig.uiElement.gameObject.SetActive(false); // Desactivar los textos que no sean del panel
                }
                else
                {
                    uiConfig.uiElement.gameObject.SetActive(true); // Mantener activos los textos del panel
                }
            }
        }
    }

    public void ForceActivatePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);  // Activar el panel inmediatamente
        }
    }


    // Método para cargar la nueva escena
    public void LoadNewGame()
    {
        // Llamar a la función ClearSaveData del GameManager
        GameManager.instance.ClearSaveData();
        //Marcar que el jugador ha iniciado una partida
        PlayerPrefs.SetInt("HasPlayedBefore", 1);
        PlayerPrefs.Save();
        //menuVideo.Pause();
        
        SceneManager.LoadScene("IntroNewGame"); // Cambia el nombre por el de tu escena
    }


    public void LoadContinueGame()
    {
        GameManager.instance.LoadGame();

        // Cambiar a la escena del juego
        SceneManager.LoadScene("La taberna del Dragon");
    }

    // Esta función se conecta al botón Exit
    public void ExitGame()
    {
        // Salir del modo Play si estamos en el editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Salir del juego si estamos en un ejecutable
        Application.Quit();
        #endif
    }

    // Esta función se conecta al botón Setting
    public void ShowSettings()
    {
        if (panelSetting != null && panelBT != null)
        {
            panelSetting.SetActive(true); // Mostrar PanelSetting
            panelBT.SetActive(false);    // Ocultar PanelBT
        }
        else
        {
            Debug.LogWarning("PanelSetting o PanelBT no están asignados en el Inspector.");
        }
    }

        // Esta función se conecta al botón Setting
    public void Volver()
    {
        if (panelSetting != null && panelBT != null)
        {
            panelSetting.SetActive(false); // Mostrar PanelSetting
            panelBT.SetActive(true);    // Ocultar PanelBT
        }
        else
        {
            Debug.LogWarning("PanelSetting o PanelBT no están asignados en el Inspector.");
        }
    }
}
