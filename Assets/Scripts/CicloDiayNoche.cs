using UnityEngine;

public class CicloDiayNoche : MonoBehaviour
{
    public float rotationSpeed = 10f; // Velocidad de rotación de la luz (grados por segundo)

    private Light directionalLight;

    void Start()
    {
        // Obtiene la referencia a la luz direccional
        directionalLight = GetComponent<Light>();

        // Asegúrate de que la luz es del tipo direccional
        if (directionalLight.type != LightType.Directional)
        {
            Debug.LogError("El objeto debe tener una luz direccional.");
        }
    }

    void Update()
    {
        // Rotar la luz direccional en el eje Y (simulando el movimiento de rotación de la Tierra)
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
    }
}
