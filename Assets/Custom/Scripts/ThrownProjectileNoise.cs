using System;
using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ThrownProjectileNoise : MonoBehaviour
{
    [Tooltip("the noise collider")]
    public SphereCollider NoiseCollider;

    public float NoiseDuration = 1f;
    public float Lifetime = 15f;
    public MMF_Player Feedbacks;
    public GameObject _view;
    public bool _isTriggered;

    protected void OnEnable()
    {
        _view.SetActive(true);
        NoiseCollider.enabled = false;
        _isTriggered = false;
    }

    protected void OnCollisionEnter(Collision other)
    {
        if (_isTriggered)
        {
            return;
        }

        _isTriggered = true;
        _view.SetActive(false);
        StartNoise();
        StartCoroutine(ResetNoise());
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(Lifetime);
        gameObject.SetActive(false);
        _isTriggered = false;
    }

    private IEnumerator ResetNoise()
    {
        yield return new WaitForSeconds(NoiseDuration);
        NoiseCollider.enabled = false;
    }

    private void StartNoise()
    {
        NoiseCollider.enabled = true;
        Feedbacks.PlayFeedbacks();
    }
}