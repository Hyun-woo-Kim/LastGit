using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        ps.Play();
    }

}
