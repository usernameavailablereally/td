using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusNoiseArea : MonoBehaviour
{
    public SphereCollider FocusZoneCollider;
    
    /*protected virtual void OnTriggerEnter(Collider collider)
    {
        _characterCrouch = collider.gameObject.MMGetComponentNoAlloc<Character>()?.FindAbility<CharacterCrouch>();
        if (_characterCrouch != null)
        {
            _characterCrouch.StartForcedCrouch();
        }
    }*/
}
