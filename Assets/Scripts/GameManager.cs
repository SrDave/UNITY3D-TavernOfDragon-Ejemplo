using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    public Vector3 directionalLightPosition;
    public Quaternion directionalLightRotation;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Transform player;
    public Transform directionalLight;
    public float saveInterval = 10f;

    private string saveFilePath;
    private bool isSavingEnabled = true; // Nueva bandera para controlar el guardado

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
    }

    private void Start()
    {
        StartCoroutine(AutoSave());
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    public void ClearSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            File.WriteAllText(saveFilePath, string.Empty); // Vaciar el archivo
            Debug.Log("Datos del archivo JSON borrados.");
        }
        else
        {
            Debug.LogWarning("No se encontró el archivo de guardado para borrar.");
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayerInScene();
        FindDirectionalLightInScene();
        LoadGame();
        Debug.Log("Datos aplicados tras cargar la escena.");
    }

    private void FindPlayerInScene()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró ningún objeto con la etiqueta 'Player' en la escena.");
        }
    }

    private void FindDirectionalLightInScene()
    {
        GameObject lightObject = GameObject.FindWithTag("DirectionalLight");
        if (lightObject != null)
        {
            directionalLight = lightObject.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró una luz direccional en la escena.");
        }
    }

    public void SaveGame()
    {
        if (!isSavingEnabled) return; // Evita guardar si la bandera está desactivada

        GameData gameData = new GameData();

        if (player != null)
        {
            gameData.playerPosition = player.position;
            gameData.playerRotation = player.rotation;
        }

        if (directionalLight != null)
        {
            gameData.directionalLightPosition = directionalLight.position;
            gameData.directionalLightRotation = directionalLight.rotation;
        }

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Juego guardado: " + json);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            if (player != null)
            {
                player.position = gameData.playerPosition;
                player.rotation = gameData.playerRotation;
            }

            if (directionalLight != null)
            {
                directionalLight.position = gameData.directionalLightPosition;
                directionalLight.rotation = gameData.directionalLightRotation;
            }
        }
    }

    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(saveInterval);
            SaveGame();
        }
    }

    public void SetSavingEnabled(bool enabled)
    {
        isSavingEnabled = enabled;
        Debug.Log($"Guardado automático {(enabled ? "activado" : "desactivado")}.");
    }
}
