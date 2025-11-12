using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFxHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private ParticleSystem ps;
    public List<ParticleSystem.Particle> particleSystems = new List<ParticleSystem.Particle>();

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
