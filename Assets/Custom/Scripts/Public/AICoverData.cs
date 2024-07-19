using UnityEngine;

namespace TD.Public
{
    public class AICoverData
    {
        public Cover CurrentCover;
        public Transform SelectedCrouchPoint;
        public bool IsReached;

        public void SelectNew(Cover cover, Transform crouchPoint)
        {
            if (CurrentCover != null)
            {
                CurrentCover.IsTaken = false;
            }

            CurrentCover = cover;
            SelectedCrouchPoint = crouchPoint;
            cover.IsTaken = true;
            IsReached = false;
        }
    }    
}
