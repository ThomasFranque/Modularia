using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartWaveParticlesControllerV3D : MonoBehaviour
{

    public Transform startLaserPoint;
    public ParticleSystem controlPS;
    public ParticleSystem distortionSpherePS;
    public AnimationCurve ac;

    private float globalProgress;
    private Renderer[] renderers;
    private ParticleSystem.Particle[] controlParticles;
    private Vector4[] controlParticlesPositions;
    private float[] controlParticlesSizes;

    void Start()
    {
        controlParticlesPositions = new Vector4[5];
        controlParticlesSizes = new float[5];
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void SetGlobalProgress(float gp)
    {
        globalProgress = gp;
    }

    // Spawn control particle and distortion sphere particle
    public void SpawnWave()
    {
        controlParticles = new ParticleSystem.Particle[5];
        distortionSpherePS.Emit(1);
        controlPS.Emit(1);
    }

    void Update()
    {
        controlPS.GetParticles(controlParticles);
        for (int i = 0; i < 5; i++)
        {
            controlParticlesPositions[i] = controlParticles[i].position;
            controlParticlesSizes[i] = controlParticles[i].GetCurrentSize(controlPS) * controlPS.transform.lossyScale.x;
        }

        // Sending position and scale to visual particle shader
        for (int i = 0; i < renderers.Length; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                renderers[i].material.SetVector("_ControlParticlePosition" + j, controlParticlesPositions[j]);
                renderers[i].material.SetFloat("_ControlParticleSize" + j, controlParticlesSizes[j]);
            }

            renderers[i].material.SetVector("_StartLaserPosition", startLaserPoint.position);
            renderers[i].material.SetFloat("_StartLaserProgress", ac.Evaluate(globalProgress));
            renderers[i].material.SetFloat("_PSLossyScale", controlPS.transform.lossyScale.x);
        }

    }
}