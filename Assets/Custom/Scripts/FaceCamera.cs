using System;
using System.Linq;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] private float _faceFrequency = 0.1f;
    private Camera _cam;

    private float _nextFaceTime;

    private void Start()
    {
        _cam = FindObjectsOfType<Camera>().First(c => c.name =="MainCamera");
    }

    private void Update()
    {
        if (Time.time >= _nextFaceTime)
        {
            transform.forward = (_cam.transform.position - transform.position).normalized;
            _nextFaceTime = Time.time + _faceFrequency;
        }
    }
}