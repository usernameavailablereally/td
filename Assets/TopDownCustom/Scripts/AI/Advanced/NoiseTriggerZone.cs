using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTriggerZone : MonoBehaviour
{
    public System.Action IsTriggered;
    private bool _readyToMakeNoise;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log($"Object Noise: Player enter detected");
            _readyToMakeNoise = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log($"Object Noise: Player exit detected");
            _readyToMakeNoise = false;
        }
    }

    private void Update()
    {
        if (_readyToMakeNoise)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log($"Object Noise: Player Click detected");
                IsTriggered?.Invoke();
            }
        }
    }
}
