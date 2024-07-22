using System;
using System.Linq;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera _cam;

    private void Start()
    {
        _cam = FindObjectsOfType<Camera>().First(c => c.name =="MainCamera");
    }

    private void Update()
    {
        transform.forward = (_cam.transform.position = transform.position).normalized;
    }
}