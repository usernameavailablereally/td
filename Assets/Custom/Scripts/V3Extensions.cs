using UnityEngine;

namespace TD.Extensions
{
    public static class V3Extensions
    {
        public static Vector2 GetXZ(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }
    }
}