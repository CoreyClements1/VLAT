using UnityEngine;
using UnityEngine.Scripting;

namespace VLAT
{
    [Preserve]
    internal static class VLATAutoBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureAccessibleRebinderAfterSceneLoad()
        {
            EnsureAccessibleRebinder("AfterSceneLoad");
        }

        private static void EnsureAccessibleRebinder(string phase)
        {
            VLATSettings settings = VLATSettings.GetOrLoad();
            if (settings == null || !settings.ShouldBootstrapAccessibleControls)
                return;

            if (Object.FindAnyObjectByType<AccessibleRebinder>() != null)
                return;

            GameObject rebinderObject = new GameObject("VLAT_AccessibleRebinder");
            rebinderObject.AddComponent<AccessibleRebinder>();
            Object.DontDestroyOnLoad(rebinderObject);

            Debug.Log($"[VLAT] AccessibleRebinder bootstrapped during {phase}.");
        }
    }
}
