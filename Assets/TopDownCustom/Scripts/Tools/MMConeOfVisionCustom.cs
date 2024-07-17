﻿using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using System;
using TD.Public;

namespace MoreMountains.Tools
{
	/// <summary>
	/// This class will create a cone of vision defined by an angle and a distance around a point. It will look for targets within that field, and draw a mesh to show the cone of vision
	/// initially inspired by this great tutorial by Sebastian Lague : https://www.youtube.com/watch?v=rQG9aUWarwE - check out his tutorials, they're amazing!
	/// </summary>
	[Serializable]
	[AddComponentMenu("More Mountains/Tools/Vision/MMConeOfVisionCustom")]
	public class MMConeOfVisionCustom : MonoBehaviour
	{
		/// <summary>
		/// A struct to store raycast data
		/// </summary>
		public struct RaycastData
		{
			public bool Hit;
			public Vector3 Point;
			public float Distance;
			public float Angle;

			public RaycastData(bool hit, Vector3 point, float distance, float angle)
			{
				Hit = hit;
				Point = point;
				Distance = distance;
				Angle = angle;
			}
		}

		public struct MeshEdgePosition
		{
			public Vector3 PointA;
			public Vector3 PointB;

			public MeshEdgePosition(Vector3 pointA, Vector3 pointB)
			{
				PointA = pointA;
				PointB = pointB;
			}
		}

		[Header("Vision")]
		public LayerMask ObstacleMask;
		public float VisionRadius = 5f;
		[Range(0f, 360f)]
		public float VisionAngle = 20f;
		[MMReadOnly]
		public Vector3 Direction;
		[MMReadOnly]
		public Vector3 EulerAngles;
		public Vector3 Offset;
		public Vector3 TargetOffset;
		public List<RaycastLine> _linesToCast;

		[Header("Target scanning")]
		public bool ShouldScanForTargets = true;
		public LayerMask TargetMask;
		public float ScanFrequencyInSeconds = 1f;
		[MMReadOnly]
		public List<Transform> VisibleTargets = new List<Transform>();
        
		[Header("Mesh")]
		public bool ShouldDrawMesh = true;
		public float MeshDensity = 0.2f;
		public int EdgePrecision = 3;
		public float EdgeThreshold = 0.5f;

		public MeshFilter VisionMeshFilter;

		protected Mesh _visionMesh;
		protected Collider[] _targetsWithinDistance; 
		protected Transform _target;
		protected Vector3 _directionToTarget;
		protected float _distanceToTarget;
		protected float _lastScanTimestamp;

		protected List<Vector3> _viewPoints = new List<Vector3>();
		protected RaycastData _oldViewCast = new RaycastData();
		protected RaycastData _viewCast = new RaycastData();
		protected Vector3[] _vertices;
		protected int[] _triangles;
		protected Vector3 _minPoint, _maxPoint, _direction;
		protected RaycastData _returnRaycastData;
		protected RaycastHit _raycastAtAngleHit;
		protected int _numberOfVerticesLastTime = 0;

		public virtual Vector3 Center { get { return this.transform.position + Offset;  } }

		protected virtual void Awake()
		{
			_visionMesh = new Mesh();
			if (ShouldDrawMesh)
			{
				VisionMeshFilter.mesh = _visionMesh;    
			}
		}

		protected virtual void LateUpdate()
		{
			if ((Time.time - _lastScanTimestamp > ScanFrequencyInSeconds) && ShouldScanForTargets)
			{
				ScanForTargets();
			}
			DrawMesh();
		}

		public virtual void SetDirectionAndAngles(Vector3 direction, Vector3 eulerAngles)
		{
			Direction = direction;
			EulerAngles = eulerAngles;
		}

		protected virtual void ScanForTargets()
		{
			_lastScanTimestamp = Time.time;
			VisibleTargets.Clear();
			_targetsWithinDistance = Physics.OverlapSphere(Center, VisionRadius, TargetMask);
			foreach (Collider collider in _targetsWithinDistance)
			{
				_target = collider.transform;
				Vector3 targetPosition = _target.position + TargetOffset;
				_directionToTarget = (targetPosition - Center).normalized;
				float currentAngle = Vector3.Angle(Direction, _directionToTarget);

				if (!(currentAngle < VisionAngle / 2f))
				{
					continue;
				}

				_distanceToTarget = Vector3.Distance(Center, targetPosition);
				bool duplicate = VisibleTargets.Contains(_target);

				if (IsUnObscured() && !duplicate)
				{
					VisibleTargets.Add(_target);
					Debug.Log($"Visual target found");
				}
				else
				{
					Ray ray = new Ray(Center, _directionToTarget * _distanceToTarget);
					Physics.Raycast(ray, out var hit);
					Debug.Log($"Visual target blocked {hit.collider.name} {hit.transform.gameObject.layer}");
				}
					
			}
		}

		protected virtual bool IsUnObscured()
		{
			LayerMask mask = ObstacleMask | TargetMask;
			for (var i = 0; i < _linesToCast.Count; i++)
			{
				RaycastLine line = _linesToCast[i];
				
				Vector3 raycastOrigin = transform.position + line.OriginOffset;
				Vector3 directionToTarget = (_target.position + line.ToOffset) - raycastOrigin;

				RaycastHit hit = MMDebug.Raycast3D(
					raycastOrigin,
					directionToTarget.normalized,
					directionToTarget.magnitude,
					mask,
					Color.blue,
					true);
				
				Debug.DrawLine(raycastOrigin, raycastOrigin + directionToTarget, Color.black, 10f);
				
				if (hit.collider != null && hit.collider.transform == _target)
				{
					return true;
				}
			}

			return false;
		}

		protected virtual void DrawMesh()
		{
			if (!ShouldDrawMesh)
			{
				return;
			}
            
			int steps = Mathf.RoundToInt(MeshDensity * VisionAngle);
			float stepsAngle = VisionAngle / steps;

			_viewPoints.Clear();

			for (int i = 0; i <= steps; i++)
			{
				float angle = stepsAngle * i + EulerAngles.y - VisionAngle / 2f;
				_viewCast = RaycastAtAngle(angle);

				if (i > 0)
				{
					bool thresholdExceeded = Mathf.Abs(_oldViewCast.Distance - _viewCast.Distance) > EdgeThreshold;

					if ((_oldViewCast.Hit != _viewCast.Hit) || (_oldViewCast.Hit && _viewCast.Hit && thresholdExceeded))
					{
						MeshEdgePosition edge = FindMeshEdgePosition(_oldViewCast, _viewCast);
						if (edge.PointA != Vector3.zero)
						{
							_viewPoints.Add(edge.PointA);
						}
						if (edge.PointB != Vector3.zero)
						{
							_viewPoints.Add(edge.PointB);
						}
					}
				}

				_viewPoints.Add(_viewCast.Point);
				_oldViewCast = _viewCast;
			}

			int numberOfVertices = _viewPoints.Count + 1;
			if (numberOfVertices != _numberOfVerticesLastTime)
			{
				Array.Resize(ref _vertices, numberOfVertices);
				Array.Resize(ref _triangles, (numberOfVertices - 2) * 3);
			}

			_vertices[0] = Offset;
			for (int i = 0; i < numberOfVertices - 1; i++)
			{
				_vertices[i + 1] = this.transform.InverseTransformPoint(_viewPoints[i]);

				if (i < numberOfVertices - 2)
				{
					_triangles[i * 3] = 0;
					_triangles[i * 3 + 1] = i + 1;
					_triangles[i * 3 + 2] = i + 2;
				}
			}

			_visionMesh.Clear();
			_visionMesh.vertices = _vertices;
			_visionMesh.triangles = _triangles;
			_visionMesh.RecalculateNormals();
            
			_numberOfVerticesLastTime = numberOfVertices;
		}

		MeshEdgePosition FindMeshEdgePosition(RaycastData minimumViewCast, RaycastData maximumViewCast)
		{
			float minAngle = minimumViewCast.Angle;
			float maxAngle = maximumViewCast.Angle;
			_minPoint = minimumViewCast.Point;
			_maxPoint = maximumViewCast.Point;

			for (int i = 0; i < EdgePrecision; i++)
			{
				float angle = (minAngle + maxAngle) / 2;
				RaycastData newViewCast = RaycastAtAngle(angle);

				bool thresholdExceeded = Mathf.Abs(minimumViewCast.Distance - newViewCast.Distance) > EdgeThreshold;
				if (newViewCast.Hit == minimumViewCast.Hit && !thresholdExceeded)
				{
					minAngle = angle;
					_minPoint = newViewCast.Point;
				}
				else
				{
					maxAngle = angle;
					_maxPoint = newViewCast.Point;
				}
			}

			return new MeshEdgePosition(_minPoint, _maxPoint);
		}

		RaycastData RaycastAtAngle(float angle)
		{
			_direction = MMMaths.DirectionFromAngle(angle, 0f);
            

			if (Physics.Raycast(Center, _direction, out _raycastAtAngleHit, VisionRadius, ObstacleMask))
			{
				_returnRaycastData.Hit = true;
				_returnRaycastData.Point = _raycastAtAngleHit.point;
				_returnRaycastData.Distance = _raycastAtAngleHit.distance;
				_returnRaycastData.Angle = angle;
			}
			else
			{
				_returnRaycastData.Hit = false;
				_returnRaycastData.Point = Center + _direction * VisionRadius;
				_returnRaycastData.Distance = VisionRadius;
				_returnRaycastData.Angle = angle;
			}

			return _returnRaycastData;
		}
	}
}