using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    public Image BarraPlayer;
    public float VidaMaxima;
    public float VidaActual;

    // Update is called once per frame
    void Update()
    {
        BarraPlayer.fillAmount = VidaActual / VidaMaxima;
    }
}
