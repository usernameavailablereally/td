using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    public class CustomTextFeedbackVisualizer : MonoBehaviour
    {
        /// the feedback to play when getting damage
        [Tooltip("the feedback to play when getting damage")]
        public MMFeedbacks DamageMMFeedbacks;

        public string CachedTextValue { get; set; }

        public void Initialize()
        {
            DamageMMFeedbacks?.Initialization(this.gameObject);
        }

        public void VisualizeDamage(string damageValue)
        {
            CachedTextValue = damageValue;
            DamageMMFeedbacks?.PlayFeedbacks(this.transform.position, Random.value);
        }
    }
}