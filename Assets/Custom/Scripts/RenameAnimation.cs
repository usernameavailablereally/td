#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Custom
{
    public class RenameAnimation : MonoBehaviour
    {
        [SerializeField] private string _from;
        [SerializeField] private string _to;
        [SerializeField] private AnimationClip _clip;

        [ContextMenu("RenameProperties")]
        void RenameProperties()
        {
            if (_clip == null)
            {
                Debug.LogError("Animation clip not set");
                return;
            }

            List<EditorCurveBinding> newBindings = new List<EditorCurveBinding>();
            List<AnimationCurve> newCurves = new List<AnimationCurve>();

            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(_clip);
            foreach (EditorCurveBinding binding in bindings)
            {
                string newPath = binding.path.Replace(_from, _to);
                
                EditorCurveBinding newBinding = new EditorCurveBinding
                {
                    path = newPath,
                    type = binding.type,
                    propertyName = binding.propertyName
                };

                AnimationCurve curve = AnimationUtility.GetEditorCurve(_clip, binding);

                newBindings.Add(newBinding);
                newCurves.Add(curve);
            }

            foreach (EditorCurveBinding binding in bindings)
            {
                AnimationUtility.SetEditorCurve(_clip, binding, null);
            }

            for (int i = 0; i < newBindings.Count; i++)
            {
                AnimationUtility.SetEditorCurve(_clip, newBindings[i], newCurves[i]);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Animation properties renamed successfully.");
        }
    }
}
#endif