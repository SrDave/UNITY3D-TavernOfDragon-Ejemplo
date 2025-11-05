using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DañoDragon : MonoBehaviour
{
    public Animator dragonAnimator; // Asigna el Animator del dragón en el Inspector
    public AudioClip Hit; // Clip de audio para el impacto
    private AudioSource audiosource; // Fuente de audio
    private Collider dragonCollider; // Collider del dragón para el raycast
    private bool isMouseOver = false; // Para verificar si el ratón está sobre el dragón

    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        dragonCollider = GetComponent<Collider>();

        if (dragonCollider == null)
        {
            Debug.LogError("El dragón necesita un Collider asignado para detectar clics.");
        }
    }

    void Update()
    {
        // Detecta clic izquierdo cuando el ratón está sobre el dragón
        if (isMouseOver && Input.GetMouseButtonDown(0))
        {
            ActivarAnimacion();
        }
    }

    void OnMouseEnter()
    {
        // Cuando el ratón pasa sobre el dragón
        isMouseOver = true;
    }

    void OnMouseExit()
    {
        // Cuando el ratón sale del dragón
        isMouseOver = false;
    }

    private void ActivarAnimacion()
    {
        // Verifica que el Animator no sea nulo antes de usarlo
        if (dragonAnimator != null)
        {
            // Activa el Trigger "ActivateNewAnimation" en el Animator
            dragonAnimator.SetTrigger("ActivateNewAnimation");
            if (audiosource != null && Hit != null)
            {
                audiosource.PlayOneShot(Hit);
            }
        }
        else
        {
            Debug.LogError("El Animator del dragón no está asignado en el script DañoDragon.");
        }
    }
}
