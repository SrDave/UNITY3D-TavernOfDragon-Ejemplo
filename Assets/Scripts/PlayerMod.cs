using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMod : MonoBehaviour
{
    public float runSpeed = 7;
    public float rotationSpeed = 250;

    public Animator animator; // Animator principal

    public Animator jumpAnimator; // Animator para los saltos

    public Animator Roll; // Animator para los saltos

    private float x, y;

    // Update is called once per frame
    void Update()
    {
        // Movimiento básico
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");



        transform.Rotate(0, x * Time.deltaTime * rotationSpeed, 0);
        transform.Translate(0, 0, y * Time.deltaTime * runSpeed);

        animator.SetFloat("VelX", x);
        animator.SetFloat("VelY", y);

        // Lógica para saltos
        if (Input.GetKeyDown(KeyCode.Space))
        {           
            jumpAnimator.SetTrigger("Jumping");   
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {           
            Roll.SetTrigger("Roll");   
        }
    }
}
