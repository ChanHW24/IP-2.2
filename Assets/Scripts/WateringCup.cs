using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringCup : MonoBehaviour
{
    public ParticleSystem particle;
    
    void Start()
    {
        var emission = particle.emission;
        emission.rateOverTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Transform transform = gameObject.transform;
        float emissionRate = Mathf.Clamp((transform.rotation.x + 20) / -60, 0, 1) * 20;

        var emission = particle.emission;
        emission.rateOverTime = emissionRate;
    }
}
