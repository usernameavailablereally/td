using UnityEngine;
using UnityEngine.UIElements;

namespace TD.Public
{
    public class Cover : MonoBehaviour, ISearchItem
    {
        [field: SerializeField] public Transform[] CrouchTransforms { get; private set; }

        public Vector3 Position => transform.position;
        public Transform CrouchTransform => transform;
        // serialized for visibility in inspector
        [field: SerializeField] public bool IsTaken { get; set; }

        public bool IsDiscoverable() => !IsTaken;
    }
}