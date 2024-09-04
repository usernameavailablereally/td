using Unity.Plastic.Antlr3.Runtime.Misc;
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