using System;
using System.Collections.Generic;
using UnityEngine;

namespace VLAT
{
    public static class VLATTween
    {
        private static readonly List<ITweenInstance> ActiveTweens = new List<ITweenInstance>();
        private static readonly Dictionary<GameObject, List<ITweenInstance>> TweensByTarget = new Dictionary<GameObject, List<ITweenInstance>>();
        private static VLATTweenRunner _runner;
        private static int _nextId = 1;

        public static TweenHandle<float> Value(float from, float to, float duration)
        {
            return CreateTween(null, from, to, duration, Mathf.Lerp);
        }

        public static TweenHandle<float> Value(GameObject target, float from, float to, float duration)
        {
            return CreateTween(target, from, to, duration, Mathf.Lerp);
        }

        public static TweenHandle<Color> Value(GameObject target, Color from, Color to, float duration)
        {
            return CreateTween(target, from, to, duration, Color.Lerp);
        }

        public static TweenHandle<Vector2> Value(GameObject target, Vector2 from, Vector2 to, float duration)
        {
            return CreateTween(target, from, to, duration, Vector2.Lerp);
        }

        public static TweenHandle<Vector3> Value(GameObject target, Vector3 from, Vector3 to, float duration)
        {
            return CreateTween(target, from, to, duration, Vector3.Lerp);
        }

        public static void Cancel(GameObject target)
        {
            if (target == null || !TweensByTarget.TryGetValue(target, out List<ITweenInstance> tweens))
                return;

            for (int i = tweens.Count - 1; i >= 0; i--)
                tweens[i].Cancel();

            tweens.Clear();
        }

        private static void Unregister(ITweenInstance tween)
        {
            if (tween.Target != null && TweensByTarget.TryGetValue(tween.Target, out List<ITweenInstance> tweens))
            {
                tweens.Remove(tween);
                if (tweens.Count == 0)
                    TweensByTarget.Remove(tween.Target);
            }
        }

        private static TweenHandle<T> CreateTween<T>(
            GameObject target,
            T from,
            T to,
            float duration,
            Func<T, T, float, T> lerp)
        {
            EnsureRunner();

            var tween = new TweenInstance<T>
            {
                Id = _nextId++,
                Target = target,
                From = from,
                To = to,
                Duration = Mathf.Max(0f, duration),
                Lerp = lerp
            };

            ActiveTweens.Add(tween);

            if (target != null)
            {
                if (!TweensByTarget.TryGetValue(target, out List<ITweenInstance> tweens))
                {
                    tweens = new List<ITweenInstance>();
                    TweensByTarget[target] = tweens;
                }

                tweens.Add(tween);
            }

            return new TweenHandle<T>(tween);
        }

        private static void EnsureRunner()
        {
            if (_runner != null)
                return;

            var runnerObject = new GameObject("[VLATTween]");
            runnerObject.hideFlags = HideFlags.HideAndDontSave;
            UnityEngine.Object.DontDestroyOnLoad(runnerObject);
            _runner = runnerObject.AddComponent<VLATTweenRunner>();
        }

        private static void Tick(float deltaTime)
        {
            for (int i = ActiveTweens.Count - 1; i >= 0; i--)
            {
                if (!ActiveTweens[i].Step(deltaTime))
                {
                    Unregister(ActiveTweens[i]);
                    ActiveTweens.RemoveAt(i);
                }
            }
        }

        internal interface ITweenInstance
        {
            GameObject Target { get; }
            float Duration { get; }
            float Delay { get; set; }
            VLATEaseType Ease { get; set; }
            bool Step(float deltaTime);
            void Cancel();
        }

        internal sealed class TweenInstance<T> : ITweenInstance
        {
            public int Id;
            public GameObject Target { get; set; }
            public float Duration { get; set; }
            public float Delay { get; set; }
            public float Elapsed;
            public VLATEaseType Ease { get; set; } = VLATEaseType.Linear;
            public bool IsCancelled;
            public T From;
            public T To;
            public Func<T, T, float, T> Lerp;
            public Action<T> OnUpdate;
            public Action OnComplete;

            public bool Step(float deltaTime)
            {
                if (IsCancelled)
                    return false;

                if (Delay > 0f)
                {
                    Delay -= deltaTime;
                    return true;
                }

                Elapsed += deltaTime;
                float progress = Duration <= 0f ? 1f : Mathf.Clamp01(Elapsed / Duration);
                float easedProgress = VLATEase.Evaluate(Ease, progress);
                OnUpdate?.Invoke(Lerp(From, To, easedProgress));

                if (progress >= 1f)
                {
                    OnComplete?.Invoke();
                    return false;
                }

                return true;
            }

            public void Cancel()
            {
                IsCancelled = true;
            }
        }

        private sealed class VLATTweenRunner : MonoBehaviour
        {
            private void Update()
            {
                Tick(Time.deltaTime);
            }
        }

        public sealed class TweenHandle<T>
        {
            private readonly TweenInstance<T> _tween;

            internal TweenHandle(TweenInstance<T> tween)
            {
                _tween = tween;
            }

            public TweenHandle<T> SetOnUpdate(Action<T> onUpdate)
            {
                _tween.OnUpdate = onUpdate;
                return this;
            }

            public TweenHandle<T> SetOnComplete(Action onComplete)
            {
                _tween.OnComplete = onComplete;
                return this;
            }

            public TweenHandle<T> SetDelay(float delay)
            {
                _tween.Delay = Mathf.Max(0f, delay);
                return this;
            }

            public TweenHandle<T> SetEase(VLATEaseType ease)
            {
                _tween.Ease = ease;
                return this;
            }

            public TweenHandle<T> SetEaseLinear() => SetEase(VLATEaseType.Linear);
            public TweenHandle<T> SetEaseInQuad() => SetEase(VLATEaseType.InQuad);
            public TweenHandle<T> SetEaseOutQuad() => SetEase(VLATEaseType.OutQuad);
            public TweenHandle<T> SetEaseInOutQuad() => SetEase(VLATEaseType.InOutQuad);
            public TweenHandle<T> SetEaseInCubic() => SetEase(VLATEaseType.InCubic);
            public TweenHandle<T> SetEaseOutCubic() => SetEase(VLATEaseType.OutCubic);
            public TweenHandle<T> SetEaseInOutCubic() => SetEase(VLATEaseType.InOutCubic);
            public TweenHandle<T> SetEaseInQuart() => SetEase(VLATEaseType.InQuart);
            public TweenHandle<T> SetEaseOutQuart() => SetEase(VLATEaseType.OutQuart);
            public TweenHandle<T> SetEaseInOutQuart() => SetEase(VLATEaseType.InOutQuart);
            public TweenHandle<T> SetEaseInQuint() => SetEase(VLATEaseType.InQuint);
            public TweenHandle<T> SetEaseOutQuint() => SetEase(VLATEaseType.OutQuint);
            public TweenHandle<T> SetEaseInOutQuint() => SetEase(VLATEaseType.InOutQuint);
            public TweenHandle<T> SetEaseInSine() => SetEase(VLATEaseType.InSine);
            public TweenHandle<T> SetEaseOutSine() => SetEase(VLATEaseType.OutSine);
            public TweenHandle<T> SetEaseInOutSine() => SetEase(VLATEaseType.InOutSine);
            public TweenHandle<T> SetEaseInExpo() => SetEase(VLATEaseType.InExpo);
            public TweenHandle<T> SetEaseOutExpo() => SetEase(VLATEaseType.OutExpo);
            public TweenHandle<T> SetEaseInOutExpo() => SetEase(VLATEaseType.InOutExpo);
            public TweenHandle<T> SetEaseInCirc() => SetEase(VLATEaseType.InCirc);
            public TweenHandle<T> SetEaseOutCirc() => SetEase(VLATEaseType.OutCirc);
            public TweenHandle<T> SetEaseInOutCirc() => SetEase(VLATEaseType.InOutCirc);
            public TweenHandle<T> SetEaseInBack() => SetEase(VLATEaseType.InBack);
            public TweenHandle<T> SetEaseOutBack() => SetEase(VLATEaseType.OutBack);
            public TweenHandle<T> SetEaseInOutBack() => SetEase(VLATEaseType.InOutBack);
            public TweenHandle<T> SetEaseInElastic() => SetEase(VLATEaseType.InElastic);
            public TweenHandle<T> SetEaseOutElastic() => SetEase(VLATEaseType.OutElastic);
            public TweenHandle<T> SetEaseInOutElastic() => SetEase(VLATEaseType.InOutElastic);
            public TweenHandle<T> SetEaseInBounce() => SetEase(VLATEaseType.InBounce);
            public TweenHandle<T> SetEaseOutBounce() => SetEase(VLATEaseType.OutBounce);
            public TweenHandle<T> SetEaseInOutBounce() => SetEase(VLATEaseType.InOutBounce);

            public void Cancel()
            {
                _tween.Cancel();
            }
        }
    }
}
