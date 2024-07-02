using UnityEngine;
using UnityEngine.UIElements;

namespace TD.Public
{
    public class Cover : MonoBehaviour
    {
        [field: SerializeField] public Transform[] CrouchTransforms { get; private set; }


        public Vector3 Position => transform.position;
        public Transform CrouchTransform => transform;
    }
}