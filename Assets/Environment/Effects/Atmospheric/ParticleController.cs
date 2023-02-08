using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] float advanceStartTime;
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem particles = GetComponent<ParticleSystem>();
        particles.Simulate(advanceStartTime);
        particles.Play();
    }
}
