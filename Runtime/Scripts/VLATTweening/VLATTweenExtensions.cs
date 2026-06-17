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

        public static VLATTween.TweenHandle<Vector3> TweenMoveLocal(this Transform transform, Vector3 to, float duration)
        {
            Vector3 from = transform.localPosition;
            return VLATTween.Value(transform.gameObject, from, to, duration)
                .SetOnUpdate(pos => transform.localPosition = pos);
        }

        public static VLATTween.TweenHandle<Vector3> TweenMoveLocalY(this Transform transform, float toY, float duration)
        {
            Vector3 from = transform.localPosition;
            Vector3 to = new Vector3(from.x, toY, from.z);
            return VLATTween.Value(transform.gameObject, from, to, duration)
                .SetOnUpdate(pos => transform.localPosition = pos);
        }

        public static VLATTween.TweenHandle<Vector3> TweenMoveLocalY(this RectTransform rectTransform, float toY, float duration)
        {
            Vector3 from = rectTransform.localPosition;
            Vector3 to = new Vector3(from.x, toY, from.z);
            return VLATTween.Value(rectTransform.gameObject, from, to, duration)
                .SetOnUpdate(pos => rectTransform.localPosition = pos);
        }
    }
}
