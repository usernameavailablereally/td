using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrajectory : MonoBehaviour
{
    [SerializeField] private LineRenderer _renderer;

    [SerializeField] private float _scrollSpeed = 1f;
    [SerializeField] private float _throwAngle = 65f;
    [SerializeField] private float _maxDistance = 20f;
    
    private Vector3[] _trajectoryPoints;

    private void Awake()
    {
        const int pointsCount = 10;
        _trajectoryPoints = new Vector3[pointsCount];
        Hide();
    }

    public void Show()
    {
        _renderer.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _renderer.gameObject.SetActive(false);
    }

    private void Update()
    {
        var offset = Time.time * _scrollSpeed;
        _renderer.material.SetTextureOffset ("_MainTex",  new Vector2(offset, offset));
    }

    public void UpdateTrajectory(Vector3 start, Vector3 end, Vector3 initialVelocity)
    {
        float flightTime = CalculateTimeOfFlight(transform.position, end, _throwAngle);
        float timeStep = flightTime / (float)(_trajectoryPoints.Length - 1);

        float time = 0;
        Vector3 currentPosition = start;
        Vector3 gravity = Physics.gravity;

        for (var i = 0; i < _trajectoryPoints.Length; i++)
        {
            _trajectoryPoints[i] = (currentPosition);
            time += timeStep;
            currentPosition = start + initialVelocity * time + 0.5f * gravity * time * time;
        }

        _renderer.positionCount = _trajectoryPoints.Length;
        _renderer.SetPositions(_trajectoryPoints);
    }

    float CalculateTimeOfFlight(Vector3 start, Vector3 end, float angle)
    {
        Vector3 displacement = end - start;
        float g = Mathf.Abs(Physics.gravity.y);
        float radianAngle = angle * Mathf.Deg2Rad;

        float horizontalDistance = new Vector3(displacement.x, 0, displacement.z).magnitude;
        float initialVelocity = Mathf.Sqrt(horizontalDistance * g / Mathf.Sin(2 * radianAngle));
        float v_y = initialVelocity * Mathf.Sin(radianAngle);
        float timeToPeak = v_y / g;
        
        float peakHeight = start.y + (v_y * timeToPeak) - (0.5f * g * timeToPeak * timeToPeak);
        float timeToFall = Mathf.Sqrt((2 * (peakHeight - end.y)) / g);
        float totalTime = timeToPeak + timeToFall;

        return totalTime;
    }

    public Vector3 CalculateForceWithAngle(Vector3 start, Vector3 end, float mass)
    {
        Vector3 displacement = end - start;
        float g = Mathf.Abs(Physics.gravity.y);
        float radianAngle = _throwAngle * Mathf.Deg2Rad;

        float horizontalDistance = new Vector3(displacement.x, 0, displacement.z).magnitude;
        float initialVelocity = Mathf.Sqrt(horizontalDistance * g / Mathf.Sin(2 * radianAngle));

        float vXZ = initialVelocity * Mathf.Cos(radianAngle);
        float vY = initialVelocity * Mathf.Sin(radianAngle);

        Vector3 direction = new Vector3(displacement.x, 0, displacement.z).normalized;
        Vector3 initialVelocityVector = direction * vXZ + Vector3.up * vY;
        Vector3 force = mass * initialVelocityVector;

        return force;
    }
}