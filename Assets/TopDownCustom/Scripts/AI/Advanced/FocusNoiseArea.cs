using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class FocusNoiseArea : MonoBehaviour
{
    public SphereCollider FocusZoneCollider;
    public AIBrain Brain;

    public Transform GetTarget()
    {
        return Brain.Target;
    }
}
