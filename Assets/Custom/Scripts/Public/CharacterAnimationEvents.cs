using System;
using UnityEngine;

namespace TD.Public
{
    public class CharacterAnimationEvents : MonoBehaviour
    {
        public event Action OnThrowFrameReached;

        public void HandleThrowFrameReached()
        {
            OnThrowFrameReached?.Invoke();
        }
    }
}