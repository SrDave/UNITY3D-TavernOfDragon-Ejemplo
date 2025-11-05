using UnityEngine;
using System.Collections;

public class ColliderEnemy : MonoBehaviour
{
    public Animator animator;
    public Transform enemigo; // Arrastra aquí el transform del enemigo
    public Transform player;  // Arrastra aquí el transform del jugador
    public GameObject barraDeVida;
    public float rotationSpeed = 5f; // Velocidad de rotación del enemigo
    private bool bronca = false;
    private bool isPlayerInOuterCollider = false; // Controla si el jugador está en el OuterCollider
    private bool isPlayerInInnerCollider = false; // Controla si el jugador está en el InnerCollider
    private bool isCoroutineRunning = false;

    public AudioSource audioSource; // Fuente de audio
    public AudioClip yellSound;     // Efecto de sonido para "Yell"
    public AudioClip sonidoBatalla;

    private void Start()
    {
        // Asegúrate de que la barra de vida esté oculta al inicio
        if (barraDeVida != null)
        {
            barraDeVida.SetActive(false);
        }
    }

    private void Update()
    {
      
        // Si Mirayo.muerto es true, inicia la corutina para desactivar la barra de vida tras 10 segundos
        if (Mirayo.muerto)
        {
            if (audioSource.isPlaying)  // Detener el audio si está reproduciéndose
            {
                audioSource.Stop();
            }
            if (!isCoroutineRunning)
            {
                StartCoroutine(DesactivarBarraDeVida());
            }
        }

        // Si el jugador está en el OuterCollider o InnerCollider, el enemigo sigue mirando al jugador
        if (isPlayerInOuterCollider || isPlayerInInnerCollider || bronca)
        {
            LookAtPlayer();
        }

        // Si 'bronca' aún no ha sido activado, comprobar si la animación 'newYell' ha terminado
        if (bronca == false)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            // Verifica si la animación 'newYell' está en ejecución y si ha terminado
            if (stateInfo.IsName("newYell") && stateInfo.normalizedTime >= 0.95f)
            {
                audioSource.clip = sonidoBatalla;
                if (!audioSource.isPlaying)
                {
                    audioSource.Stop();
                    audioSource.Play();
                }
                bronca = true; // Marca que la bronca ha sido activada
                Debug.Log("Animación newYell terminada, activando bronca.");
                // Muestra la barra de vida si está en el OuterCollider y bronca es verdadero
                if (isPlayerInOuterCollider && barraDeVida != null)
                {
                    barraDeVida.SetActive(true);
                }
            }
            
        }
    }

    // Corutina para desactivar la barra de vida después de 10 segundos
    private IEnumerator DesactivarBarraDeVida()
    {
        isCoroutineRunning = true; // Marca la corutina como en ejecución
        yield return new WaitForSeconds(5); // Espera 10 segundos
        if (barraDeVida != null)
        {
            barraDeVida.SetActive(false); // Desactiva la barra de vida
        }
        isCoroutineRunning = false; // Marca la corutina como finalizada
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Mirayo.muerto) return;

        if (other.CompareTag("OuterCollider"))
        {   
            if (!bronca) // Solo se activa si bronca es falso
            {
                isPlayerInOuterCollider = true;
                animator.SetTrigger("Yell");  // Activa la animación "newYell"
                // Asignar y reproducir el sonido
                audioSource.clip = yellSound;
                audioSource.Play();
                Debug.Log("Entered OuterCollider: Trigger 'yell' activated");
            }
            else
            {
                audioSource.Stop();
                audioSource.clip = sonidoBatalla;
                if (!audioSource.isPlaying)
                {
                    audioSource.Stop();
                    audioSource.Play();
                }
                barraDeVida.SetActive(true);
                animator.SetTrigger("Bronca"); // Reproduce la animación "Bronca" cuando bronca es verdadero
            }
        }
        else if (other.CompareTag("InnerCollider"))
        {
            isPlayerInInnerCollider = true;
            barraDeVida.SetActive(true);
            
            audioSource.clip = sonidoBatalla;
            if (!audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.Play();
            }
            animator.SetTrigger("SaltoBack");
            LookAtPlayer();
            Debug.Log("Entered InnerCollider: Trigger 'SaltoBack' activated");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Mirayo.muerto) return;

        if (other.CompareTag("OuterCollider"))
        {
            audioSource.Stop();
            isPlayerInOuterCollider = false;
            barraDeVida.SetActive(false);
            animator.SetTrigger("exit");
            Debug.Log("Exited OuterCollider: Returning to default animation");
        }
        else if (other.CompareTag("InnerCollider"))
        {
            isPlayerInInnerCollider = false;
        }
    }

    private void LookAtPlayer()
    {
        if (Mirayo.muerto) return;
        if (enemigo == null || player == null) return;

        // Dirección hacia el jugador
        Vector3 direction = player.position - enemigo.position;
        direction.y = 0; // Mantener la rotación en el eje horizontal

        // Calcula la rotación hacia el jugador
        if (direction.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            enemigo.rotation = Quaternion.Slerp(enemigo.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
