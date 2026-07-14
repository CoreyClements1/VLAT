using UnityEngine;

namespace VLAT
{
    /// <summary>
    /// Project-wide VLAT configuration. Edit via menu bar → VERA → VLAT Settings.
    /// Loaded at runtime from a Resources asset named "VLATSettings"
    /// (typically Assets/VERA/Resources/VLATSettings.asset).
    /// </summary>
    [CreateAssetMenu(fileName = "VLATSettings", menuName = "VERA/VLAT Settings", order = 0)]
    public class VLATSettings : ScriptableObject
    {
        public const string ResourceName = "VLATSettings";

        [Header("Accessible Controls")]
        [Tooltip("Whether this project supports VLAT accessible controls. When disabled, VLAT will not auto-bootstrap accessibility systems.")]
        public bool supportsAccessibleControls = false;

        [Tooltip("For testing: always bootstrap accessible controls. Still tries to load participant settings; if that fails, defaults are used and controls remain enabled.")]
        public bool alwaysEnableAccessibleControlsForTesting = false;

        /// <summary>
        /// Whether to show the accessibility setup screen on launch.
        /// Temporarily forced off while that feature is in development; wiring remains in place.
        /// </summary>
        public bool showSetupOnStart => false;

        /// <summary>
        /// True when accessible controls should be bootstrapped for this project.
        /// </summary>
        public bool ShouldBootstrapAccessibleControls =>
            supportsAccessibleControls || alwaysEnableAccessibleControlsForTesting;

        [Header("XR Player")]
        [Tooltip("The radius of the player's movement collider")]
        public float xrPlayerRadius = 0.2f;

        [Tooltip("The height of the player's movement collider")]
        public float xrPlayerHeight = 2.0f;

        [Header("Movement")]
        [Tooltip("The player's default movement speed, when using VLAT movement")]
        public float playerMoveSpeed = 10f;

        [Header("Interaction")]
        [Tooltip("The maximum distance from which interactable objects may be interacted with")]
        public float interactionRadius = 10f;

        [Header("Virtual Hands")]
        [HideInInspector]
        [Tooltip("Whether the player has virtual hands present while playing")]
        public bool playerHasVirtualHands = true;

        private static VLATSettings cachedInstance;

        /// <summary>
        /// Loads the project VLAT settings asset from Resources, or returns an in-memory default instance.
        /// </summary>
        public static VLATSettings GetOrLoad()
        {
            if (cachedInstance != null)
                return cachedInstance;

            // Prefer Resources at runtime. Never call AssetDatabase while entering/playing —
            // that can freeze the Unity Editor (busy spinner, play mode never starts).
            cachedInstance = Resources.Load<VLATSettings>(ResourceName);

#if UNITY_EDITOR
            if (cachedInstance == null && !Application.isPlaying)
            {
                cachedInstance = FindPreferredSettingsAssetInEditor();
                if (cachedInstance != null)
                    return cachedInstance;
            }
#endif

            if (cachedInstance == null)
            {
                Debug.LogWarning(
                    "VLATSettings: No Resources/VLATSettings asset found. Using default values. " +
                    "Open VERA → VLAT Settings to create and configure one.");
                cachedInstance = CreateInstance<VLATSettings>();
            }

            return cachedInstance;
        }

        /// <summary>
        /// Clears the cached instance (e.g. after the asset is recreated in the editor).
        /// </summary>
        public static void ClearCache()
        {
            cachedInstance = null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Assigns the active settings instance used by the editor window and runtime cache.
        /// </summary>
        public static void SetCachedInstance(VLATSettings settings)
        {
            cachedInstance = settings;
        }

        private static VLATSettings FindPreferredSettingsAssetInEditor()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:VLATSettings");
            VLATSettings resourcesFallback = null;

            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                VLATSettings candidate = UnityEditor.AssetDatabase.LoadAssetAtPath<VLATSettings>(path);
                if (candidate == null)
                    continue;

                // Prefer project Resources so runtime Resources.Load matches editor selection.
                if (path.StartsWith("Assets/") && path.Contains("/Resources/"))
                    return candidate;

                if (path.StartsWith("Assets/") && resourcesFallback == null)
                    resourcesFallback = candidate;
            }

            return resourcesFallback;
        }
#endif
    }
}
