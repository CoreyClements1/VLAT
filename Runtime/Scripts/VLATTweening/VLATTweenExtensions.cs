using UnityEngine;

namespace VLAT
{
    public static class VLATTweenExtensions
    {
        public static VLATTween.TweenHandle<float> TweenAlpha(this CanvasGroup canvasGroup, float to, float duration)
        {
            return VLATTween.Value(canvasGroup.gameObject, canvasGroup.alpha, to, duration)
                .SetOnUpdate(alpha => canvasGroup.alpha = alpha);
        }
    }
}
