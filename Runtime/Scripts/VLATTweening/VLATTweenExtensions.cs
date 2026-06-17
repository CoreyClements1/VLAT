using UnityEngine;

namespace VLAT
{
    public static class VLATTweenExtensions
    {
        public static VLATTween.TweenHandle<float> TweenAlpha(this CanvasGroup canvasGroup, float to, float duration)
        {
            GameObject target = canvasGroup.gameObject;
            return VLATTween.Value(target, canvasGroup.alpha, to, duration)
                .SetOnUpdate(alpha =>
                {
                    if (target == null)
                        return;

                    CanvasGroup group = target.GetComponent<CanvasGroup>();
                    if (group != null)
                        group.alpha = alpha;
                });
        }

        public static VLATTween.TweenHandle<Vector3> TweenMoveLocal(this Transform transform, Vector3 to, float duration)
        {
            GameObject target = transform.gameObject;
            Vector3 from = transform.localPosition;
            return VLATTween.Value(target, from, to, duration)
                .SetOnUpdate(pos =>
                {
                    if (target == null)
                        return;

                    target.transform.localPosition = pos;
                });
        }

        public static VLATTween.TweenHandle<Vector3> TweenMoveLocalY(this Transform transform, float toY, float duration)
        {
            GameObject target = transform.gameObject;
            Vector3 from = transform.localPosition;
            Vector3 to = new Vector3(from.x, toY, from.z);
            return VLATTween.Value(target, from, to, duration)
                .SetOnUpdate(pos =>
                {
                    if (target == null)
                        return;

                    target.transform.localPosition = pos;
                });
        }

        public static VLATTween.TweenHandle<Vector3> TweenMoveLocalY(this RectTransform rectTransform, float toY, float duration)
        {
            GameObject target = rectTransform.gameObject;
            Vector3 from = rectTransform.localPosition;
            Vector3 to = new Vector3(from.x, toY, from.z);
            return VLATTween.Value(target, from, to, duration)
                .SetOnUpdate(pos =>
                {
                    if (target == null)
                        return;

                    RectTransform rect = target.transform as RectTransform;
                    if (rect != null)
                        rect.localPosition = pos;
                });
        }
    }
}
