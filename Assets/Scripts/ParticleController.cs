using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public LineRenderer lineRenderer;

    public void PlayParticles()
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }

    public void StopParticles()
    {
        if (particleSystem != null)
        {
            particleSystem.Stop();
        }
    }

    public void EnableLineRenderer()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
        }
    }

    public void DisableLineRenderer()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }
}
