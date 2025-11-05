using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirayo : MonoBehaviour
{
    public Transform final; // Punto final del rayo
    public int cantidadDePuntos; // Puntos de la línea del rayo (opcional para efectos visuales)
    public float dispersion; // Desviación aleatoria (opcional para efectos visuales)
    public float frecuencia; // Frecuencia de actualización (opcional para efectos visuales)
    public LayerMask enemyLayer; // LayerMask para detectar solo enemigos
    public static bool muerto = false;
    private LineRenderer line;
    private float tiempo = 0;

    public AudioSource audioSource; // Fuente de audio
    public AudioClip sonidoMuerto;     // Efecto de sonido para "Yell"

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (muerto)
        {
            
        }
        tiempo += Time.deltaTime;

        if (tiempo > frecuencia)
        {
            ActualizarPuntos(line);
            tiempo = 0;

            // Lanza un rayo desde el origen hacia el final
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (final.position - transform.position).normalized, out hit, Mathf.Infinity, enemyLayer))
            {
                Debug.Log("Rayo impactó en: " + hit.collider.name);

                // Activa el trigger en el Animator del enemigo si tiene el tag "Enemy"
                if (hit.collider.CompareTag("Enemy"))
                {
                    BarraVida barraVida = hit.collider.GetComponent<BarraVida>();
                    Animator enemyAnimator = hit.collider.GetComponent<Animator>();
                    if (enemyAnimator != null)
                    {
                        StartCoroutine(ReducirVidaProgresivamente(barraVida, 100f, 5f)); // Reducir 100 puntos en 1 segundo
                        Debug.Log("Vida del enemigo reducida.");
                        audioSource.clip = sonidoMuerto;
                        audioSource.Play();
                        enemyAnimator.SetTrigger("Morir");
                        muerto = true;
                        Debug.Log("Trigger 'morir' activado en el enemigo.");
                    }
                    else
                    {
                        Debug.LogWarning("No se encontró un Animator en el enemigo.");
                    }
                }
            }
        }
    }

    private IEnumerator ReducirVidaProgresivamente(BarraVida barraVida, float cantidad, float duracion)
    {
        float cantidadPorSegundo = cantidad / duracion;
        float vidaReducida = 0;

        while (vidaReducida < cantidad && barraVida.VidaActual > 0)
        {
            float reduccion = cantidadPorSegundo * Time.deltaTime;
            barraVida.VidaActual -= reduccion;
            vidaReducida += reduccion;
            yield return null; // Espera al siguiente frame
        }

        // Asegura que la vida no sea menor a 0
        barraVida.VidaActual = Mathf.Max(0, barraVida.VidaActual);
    }


    private void ActualizarPuntos(LineRenderer line)
    {
        List<Vector3> puntos = InterpolarPuntos(Vector3.zero, final.localPosition, cantidadDePuntos);
        line.positionCount = puntos.Count;
        line.SetPositions(puntos.ToArray());
    }

    private List<Vector3> InterpolarPuntos(Vector3 principio, Vector3 final, int totalPoints)
    {
        List<Vector3> list = new List<Vector3>();

        for (int i = 0; i < totalPoints; i++)
        {
            list.Add(Vector3.Lerp(principio, final, (float)i / totalPoints) + DesfaseAleatorio());
        }
        return list;
    }

    private Vector3 DesfaseAleatorio()
    {
        return Random.insideUnitSphere.normalized * Random.Range(0, dispersion);
    }
}
