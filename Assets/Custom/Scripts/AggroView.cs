using UnityEngine;

public class AggroView : MonoBehaviour
{
    [SerializeField] private Transform _root;

    public void SetValue(float value)
    {
        Vector3 scale = _root.localScale;
        scale.y = value;
        _root.localScale = scale;
    }
}