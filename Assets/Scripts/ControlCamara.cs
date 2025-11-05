using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCamara : MonoBehaviour
{
    public float sensitivity = 100f; // Sensibilidad del ratón
    
    public Transform playerBody; // El cuerpo del jugador para rotarlo en el eje Y

    private float xRotation = 0f; // Para limitar la rotación vertical
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        // Obtiene el movimiento del ratón
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // Controla la rotación vertical de la cámara
        xRotation -= mouseY;
        //xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limita la rotación entre -90 y 90 grados

        // Aplica la rotación vertical
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rota el cuerpo del jugador en el eje Y (horizontal)
        playerBody.Rotate(Vector3.up * mouseX);
        
    }
}

