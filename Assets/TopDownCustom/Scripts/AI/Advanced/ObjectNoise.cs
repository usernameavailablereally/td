using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectNoise : MonoBehaviour
{
    [Tooltip("the noise collider")]
    public SphereCollider NoiseCollider; 
    [Tooltip("the noise sound")]
    public AudioSource NoiseAudioSource; 
    [Tooltip("Target Noise level when activated")]
    public float TargetNoiseLevel; 
    private bool _readyToMakeNoise;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log($"Object Noise Player enter detected");
            _readyToMakeNoise = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log($"Object Noise Player exit detected");
            _readyToMakeNoise = false;
        }
    }

    private void Update()
    {
        if (_readyToMakeNoise)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SetNoiseLevel(TargetNoiseLevel);
            }
        }
    }

    private void SetNoiseLevel(float level)
    {
        Debug.Log($"Object Noise SetNoiseLevel {level}");
        NoiseCollider.enabled = level > 0;
        NoiseCollider.radius = level;
        if (level > 0)
        {
            NoiseAudioSource.Play();
        }
        else
        {
            NoiseAudioSource.Stop();
        }
    }
}
