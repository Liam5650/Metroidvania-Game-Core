using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] float advanceStartTime;    // Starts the simulation at an advanced time so particles are already spawned

    void Start()
    {
        // Start the particle system
        ParticleSystem particles = GetComponent<ParticleSystem>();
        particles.Simulate(advanceStartTime);
        particles.Play();
    }
}
