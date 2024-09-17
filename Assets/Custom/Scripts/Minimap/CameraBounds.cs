using System;
using TD.Extensions;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    
    private Vector2 _min;
    private Vector2 _max;

    private void Awake()
    {
        Transform camTransform = _cam.transform;
        float orthographicSize = _cam.orthographicSize;
        float aspect = _cam.aspect;

        float height = 2.0f * orthographicSize;
        float width = height * aspect;

        Vector2 sizeXY = new Vector2(width, height);
        Vector3 camPosition = camTransform.position;

        Vector3 sizeProjected = camTransform.TransformVector(sizeXY);
        _min = (camPosition - sizeProjected / 2).GetXZ();
        _max = (camPosition + sizeProjected / 2).GetXZ();
    }

    public Vector2 GetRelativePositionPercent(Vector3 vec)
    {
        return Vector2.zero;
    }
}