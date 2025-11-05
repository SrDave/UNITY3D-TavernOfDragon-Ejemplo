using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueRayo : MonoBehaviour
{
    Animator animator;
    public AudioClip Hechizo;
    AudioSource audiosource;

    // Start is called before the first frame update
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Detecta si se presiona la tecla "E"
        if (Input.GetKeyDown(KeyCode.E))
        {
            RayoParticulas();
        }
    }

    void RayoParticulas()
    {
        // Activa el trigger para la animación del rayo
        animator.SetTrigger("RayoParticulas");
        audiosource.PlayOneShot(Hechizo);
    }
}
