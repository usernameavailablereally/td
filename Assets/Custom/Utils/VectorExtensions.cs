using UnityEngine;

namespace TD.Extensions
{
    public static class VectorExtensions
    {
        private const float nearZeroNumber = 0.000001f;

        public static Vector2 GetXZ(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }
        
        public static void RotateVectorAroundPivot(ref Vector2 pointA, ref Vector2 pointB, Vector2 pivot, float angleDegrees)
        {
            // Rotate both points of the vector around the pivot
            pointA = RotateAroundPivot(pointA, pivot, angleDegrees);
            pointB = RotateAroundPivot(pointB, pivot, angleDegrees);
        }

        public static Vector2 RotateAroundPivot(this Vector2 point, Vector2 pivot, float angleDegrees)
        {
            float angleRadians = angleDegrees * Mathf.Deg2Rad;

            Vector2 direction = point - pivot;

            float cosTheta = Mathf.Cos(angleRadians);
            float sinTheta = Mathf.Sin(angleRadians);

            Vector2 rotatedDirection = new Vector2(
                cosTheta * direction.x - sinTheta * direction.y,
                sinTheta * direction.x + cosTheta * direction.y
            );

            Vector2 rotatedPoint = rotatedDirection + pivot;

            return rotatedPoint;
        }

        public static Vector2 MultiplyAxiswise(this Vector2 vec, Vector2 vec2)
        {
            return new Vector2(vec.x * vec2.x, vec.y * vec2.y);
        }        
        
        public static Vector2 DivideAxiswise(this Vector2 vec, Vector2 vec2, bool allowZero = true)
        {
            if (allowZero)
            {
                if (vec2.x == 0)
                {
                    vec2.x = nearZeroNumber;
                }

                if (vec2.y == 0)
                {
                    vec2.y = nearZeroNumber;
                }
            }
            else
            {
                Debug.Assert(vec2.x > 0 && vec2.y > 0, "Vector has zero components during division");
            }

            return new Vector2(vec.x / vec2.x, vec.y / vec2.y);
        }
        
        public static Vector2 ClampInRotatedRectangle(this Vector2 point, Vector2 rectCenter, float rectWidth, float rectHeight, float rotationDegrees)
        {
            Vector2 rotatedPoint = RotateAroundPivot(point, rectCenter, -rotationDegrees);

            float halfWidth = rectWidth / 2.0f;
            float halfHeight = rectHeight / 2.0f;
            float minX = rectCenter.x - halfWidth;
            float maxX = rectCenter.x + halfWidth;
            float minY = rectCenter.y - halfHeight;
            float maxY = rectCenter.y + halfHeight;

            float clampedX = Mathf.Clamp(rotatedPoint.x, minX, maxX);
            float clampedY = Mathf.Clamp(rotatedPoint.y, minY, maxY);

            Vector2 clampedPoint = new Vector2(clampedX, clampedY);
            return RotateAroundPivot(clampedPoint, rectCenter, rotationDegrees);
        }
    }
}