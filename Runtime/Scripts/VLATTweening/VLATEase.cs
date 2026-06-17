using UnityEngine;

namespace VLAT
{
    public enum VLATEaseType
    {
        Linear,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InSine,
        OutSine,
        InOutSine,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InBack,
        OutBack,
        InOutBack,
        InElastic,
        OutElastic,
        InOutElastic,
        InBounce,
        OutBounce,
        InOutBounce
    }

    public static class VLATEase
    {
        private const float BackOvershoot = 1.70158f;
        private const float TwoPi = Mathf.PI * 2f;
        private const float HalfPi = Mathf.PI * 0.5f;

        public static float Evaluate(VLATEaseType ease, float t)
        {
            t = Mathf.Clamp01(t);

            switch (ease)
            {
                case VLATEaseType.Linear:
                    return t;
                case VLATEaseType.InQuad:
                    return t * t;
                case VLATEaseType.OutQuad:
                    return 1f - (1f - t) * (1f - t);
                case VLATEaseType.InOutQuad:
                    return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;
                case VLATEaseType.InCubic:
                    return t * t * t;
                case VLATEaseType.OutCubic:
                    return 1f - Mathf.Pow(1f - t, 3f);
                case VLATEaseType.InOutCubic:
                    return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;
                case VLATEaseType.InQuart:
                    return t * t * t * t;
                case VLATEaseType.OutQuart:
                    return 1f - Mathf.Pow(1f - t, 4f);
                case VLATEaseType.InOutQuart:
                    return t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) * 0.5f;
                case VLATEaseType.InQuint:
                    return t * t * t * t * t;
                case VLATEaseType.OutQuint:
                    return 1f - Mathf.Pow(1f - t, 5f);
                case VLATEaseType.InOutQuint:
                    return t < 0.5f ? 16f * t * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 5f) * 0.5f;
                case VLATEaseType.InSine:
                    return 1f - Mathf.Cos(t * HalfPi);
                case VLATEaseType.OutSine:
                    return Mathf.Sin(t * HalfPi);
                case VLATEaseType.InOutSine:
                    return -(Mathf.Cos(Mathf.PI * t) - 1f) * 0.5f;
                case VLATEaseType.InExpo:
                    return t <= 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f);
                case VLATEaseType.OutExpo:
                    return t >= 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
                case VLATEaseType.InOutExpo:
                    if (t <= 0f) return 0f;
                    if (t >= 1f) return 1f;
                    return t < 0.5f
                        ? Mathf.Pow(2f, 20f * t - 10f) * 0.5f
                        : (2f - Mathf.Pow(2f, -20f * t + 10f)) * 0.5f;
                case VLATEaseType.InCirc:
                    return 1f - Mathf.Sqrt(1f - t * t);
                case VLATEaseType.OutCirc:
                    return Mathf.Sqrt(1f - Mathf.Pow(t - 1f, 2f));
                case VLATEaseType.InOutCirc:
                    return t < 0.5f
                        ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * t, 2f))) * 0.5f
                        : (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f) * 0.5f;
                case VLATEaseType.InBack:
                    return (BackOvershoot + 1f) * t * t * t - BackOvershoot * t * t;
                case VLATEaseType.OutBack:
                    return 1f + (BackOvershoot + 1f) * Mathf.Pow(t - 1f, 3f) + BackOvershoot * Mathf.Pow(t - 1f, 2f);
                case VLATEaseType.InOutBack:
                    return t < 0.5f
                        ? (Mathf.Pow(2f * t, 2f) * ((BackOvershoot * 1.525f + 1f) * 2f * t - BackOvershoot * 1.525f)) * 0.5f
                        : (Mathf.Pow(2f * t - 2f, 2f) * ((BackOvershoot * 1.525f + 1f) * (2f * t - 2f) + BackOvershoot * 1.525f) + 2f) * 0.5f;
                case VLATEaseType.InElastic:
                    if (t <= 0f) return 0f;
                    if (t >= 1f) return 1f;
                    return -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * TwoPi / 3f);
                case VLATEaseType.OutElastic:
                    if (t <= 0f) return 0f;
                    if (t >= 1f) return 1f;
                    return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * TwoPi / 3f) + 1f;
                case VLATEaseType.InOutElastic:
                    if (t <= 0f) return 0f;
                    if (t >= 1f) return 1f;
                    return t < 0.5f
                        ? -(Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin((20f * t - 11.125f) * TwoPi / 4.5f)) * 0.5f
                        : (Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin((20f * t - 11.125f) * TwoPi / 4.5f)) * 0.5f + 1f;
                case VLATEaseType.InBounce:
                    return 1f - Evaluate(VLATEaseType.OutBounce, 1f - t);
                case VLATEaseType.OutBounce:
                    const float n1 = 7.5625f;
                    const float d1 = 2.75f;
                    if (t < 1f / d1) return n1 * t * t;
                    if (t < 2f / d1) return n1 * (t -= 1.5f / d1) * t + 0.75f;
                    if (t < 2.5f / d1) return n1 * (t -= 2.25f / d1) * t + 0.9375f;
                    return n1 * (t -= 2.625f / d1) * t + 0.984375f;
                case VLATEaseType.InOutBounce:
                    return t < 0.5f
                        ? (1f - Evaluate(VLATEaseType.OutBounce, 1f - 2f * t)) * 0.5f
                        : (1f + Evaluate(VLATEaseType.OutBounce, 2f * t - 1f)) * 0.5f;
                default:
                    return t;
            }
        }
    }
}
