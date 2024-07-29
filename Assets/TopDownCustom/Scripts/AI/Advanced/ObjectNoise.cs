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

    public NoiseTriggerZone NoiseTriggerZone;

    private static bool isPlaying;

    void Awake()
    {
        NoiseTriggerZone.IsTriggered += IsZoneTriggered;
    }

    private void IsZoneTriggered()
    {
        SetNoiseLevel(TargetNoiseLevel);
    }

    private void SetNoiseLevel(float level)
    {
        Debug.Log($"Object Noise SetNoiseLevel {level}");
        NoiseCollider.enabled = level > 0;
        NoiseCollider.radius = level;
        if (level > 0 && !isPlaying)
        {
            NoiseAudioSource.Play();
            isPlaying = true;
        }
        else
        {
            NoiseAudioSource.Stop();
            isPlaying = false;
        }
    }
}
