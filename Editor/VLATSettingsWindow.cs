#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VLAT
{
    /// <summary>
    /// Global editor window for configuring VLAT accessibility / locomotion options.
    /// Opened from menu bar → VERA → VLAT Settings.
    /// </summary>
    internal class VLATSettingsWindow : EditorWindow
    {
        private const string ProjectSettingsFolder = "Assets/VERA/Resources";
        private const string ProjectSettingsAssetPath = ProjectSettingsFolder + "/VLATSettings.asset";
        private const string PackageDefaultsFilter = "t:VLATSettings";

        private VLATSettings settings;
        private SerializedObject serializedSettings;
        private Vector2 scrollPosition = Vector2.zero;

        private bool locomotionFoldout = true;

        private GUIStyle windowPaddingStyle;
        private GUIStyle headerTitleStyle;
        private GUIStyle headerSubtitleStyle;
        private GUIStyle sectionCardStyle;
        private GUIStyle footerPathStyle;

        [MenuItem("VERA/VLAT Settings")]
        public static void ShowWindow()
        {
            VLATSettingsWindow window = GetWindow<VLATSettingsWindow>("VLAT Settings");
            window.minSize = new Vector2(440, 420);
            window.Show();
        }

        private void OnEnable()
        {
            LoadOrCreateSettings();
        }

        private void OnFocus()
        {
            if (settings == null)
                LoadOrCreateSettings();
        }


        #region ON GUI


        private void OnGUI()
        {
            EnsureStyles();

            if (settings == null)
            {
                EditorGUILayout.BeginVertical(windowPaddingStyle);
                DrawHeader();
                EditorGUILayout.HelpBox(
                    "Could not load or create VLAT settings. Click the button below to retry.",
                    MessageType.Error);
                GUILayout.Space(8);
                if (GUILayout.Button("Create VLAT Settings Asset", GUILayout.Height(28)))
                    LoadOrCreateSettings();
                EditorGUILayout.EndVertical();
                return;
            }

            if (serializedSettings == null || serializedSettings.targetObject != settings)
                serializedSettings = new SerializedObject(settings);

            serializedSettings.Update();

            float previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 220f;

            EditorGUILayout.BeginVertical(windowPaddingStyle);
            DrawHeader();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawGlobalOptions();
            GUILayout.Space(14);
            DrawLocomotionSection();
            GUILayout.Space(8);

            EditorGUILayout.EndScrollView();

            DrawSeparator();
            GUILayout.Space(8);
            DrawAssetFooter();
            EditorGUILayout.EndVertical();

            EditorGUIUtility.labelWidth = previousLabelWidth;

            if (serializedSettings.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(settings);
                VLATSettings.SetCachedInstance(settings);
            }
        }


        #endregion


        #region SECTIONS


        private void DrawHeader()
        {
            GUILayout.Label("VLAT Settings", headerTitleStyle);
            GUILayout.Space(2);
            GUILayout.Label(
                "Project-wide locomotion and accessibility options. XR Origin and controller hands are resolved automatically.",
                headerSubtitleStyle);
            GUILayout.Space(6);
            DrawSeparator();
            GUILayout.Space(6);
        }

        private void DrawGlobalOptions()
        {
            EditorGUILayout.BeginVertical(sectionCardStyle);

            DrawToggleRow(
                "supportsAccessibleControls",
                "Supports Accessible Controls",
                "When enabled, VLAT auto-bootstraps accessible control systems (such as AccessibleRebinder) on scene load. " +
                "Disable for projects that do not support accessible controls.");

            GUILayout.Space(6);

            DrawToggleRow(
                "alwaysEnableAccessibleControlsForTesting",
                "Always Enable for Testing",
                "For testing: always spawn accessible controls. Still attempts to fetch participant settings; " +
                "if that fails or switch controls are disabled for the participant, default settings are used instead.");

            EditorGUILayout.EndVertical();
        }

        private void DrawLocomotionSection()
        {
            locomotionFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(locomotionFoldout, "Locomotion & Interaction");
            if (locomotionFoldout)
            {
                EditorGUILayout.BeginVertical(sectionCardStyle);

                DrawFloatRow(
                    "xrPlayerRadius",
                    "Player Radius",
                    "Radius of the XR player's movement collider. The XR Origin parent is resolved automatically from the scene.");
                GUILayout.Space(2);
                DrawFloatRow(
                    "xrPlayerHeight",
                    "Player Height",
                    "Height of the XR player's movement collider.");

                GUILayout.Space(10);
                DrawFloatRow(
                    "playerMoveSpeed",
                    "Move Speed",
                    "Default locomotion speed when using VLAT movement. Typical comfortable values are between 1 and 5.");
                GUILayout.Space(2);
                DrawFloatRow(
                    "interactionRadius",
                    "Interaction Radius",
                    "Maximum distance at which interactable objects can be selected. " +
                    "Increase for large environments; decrease to reduce accidental selections.");

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawAssetFooter()
        {
            string assetPath = AssetDatabase.GetAssetPath(settings);
            bool isProjectResourcesAsset = assetPath.StartsWith("Assets/") && assetPath.Contains("/Resources/");

            if (!isProjectResourcesAsset)
            {
                EditorGUILayout.HelpBox(
                    "Settings are not yet saved to your project. Create a project asset so VLAT can load them at runtime.",
                    MessageType.Warning);
                GUILayout.Space(6);
                if (GUILayout.Button("Create Project Settings Asset", GUILayout.Height(28)))
                {
                    settings = CreateProjectSettingsAsset(settings);
                    serializedSettings = settings != null ? new SerializedObject(settings) : null;
                    VLATSettings.SetCachedInstance(settings);
                }
                return;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(assetPath, footerPathStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Select Asset", GUILayout.Width(100), GUILayout.Height(22)))
            {
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawToggleRow(string propertyName, string label, string tooltip)
        {
            EditorGUILayout.PropertyField(
                serializedSettings.FindProperty(propertyName),
                new GUIContent(label, tooltip));
        }

        private void DrawFloatRow(string propertyName, string label, string tooltip)
        {
            EditorGUILayout.PropertyField(
                serializedSettings.FindProperty(propertyName),
                new GUIContent(label, tooltip));
        }

        private void DrawSeparator()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1f);
            EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin
                ? new Color(1f, 1f, 1f, 0.1f)
                : new Color(0f, 0f, 0f, 0.15f));
        }

        private void EnsureStyles()
        {
            if (windowPaddingStyle != null)
                return;

            windowPaddingStyle = new GUIStyle
            {
                padding = new RectOffset(14, 14, 12, 12)
            };

            headerTitleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 15,
                margin = new RectOffset(0, 0, 0, 0)
            };

            headerSubtitleStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 11,
                normal = { textColor = EditorGUIUtility.isProSkin
                    ? new Color(0.75f, 0.75f, 0.75f)
                    : new Color(0.35f, 0.35f, 0.35f) }
            };

            sectionCardStyle = new GUIStyle("helpBox")
            {
                padding = new RectOffset(12, 12, 4, 6),
                margin = new RectOffset(0, 0, 0, 0)
            };

            footerPathStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                wordWrap = true,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = EditorGUIUtility.isProSkin
                    ? new Color(0.65f, 0.65f, 0.65f)
                    : new Color(0.4f, 0.4f, 0.4f) }
            };
        }


        #endregion


        #region LOAD / CREATE


        private void LoadOrCreateSettings()
        {
            settings = FindProjectResourcesSettings();

            if (settings == null)
                settings = CreateProjectSettingsAsset(FindPackageDefaultSettings());

            if (settings != null)
            {
                serializedSettings = new SerializedObject(settings);
                VLATSettings.SetCachedInstance(settings);
            }
        }

        private static VLATSettings FindProjectResourcesSettings()
        {
            string[] guids = AssetDatabase.FindAssets(PackageDefaultsFilter);

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.StartsWith("Assets/") || !path.Contains("/Resources/"))
                    continue;

                VLATSettings candidate = AssetDatabase.LoadAssetAtPath<VLATSettings>(path);
                if (candidate != null)
                    return candidate;
            }

            return null;
        }

        private static VLATSettings FindPackageDefaultSettings()
        {
            string[] guids = AssetDatabase.FindAssets(PackageDefaultsFilter);

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.StartsWith("Assets/"))
                    continue;

                VLATSettings candidate = AssetDatabase.LoadAssetAtPath<VLATSettings>(path);
                if (candidate != null)
                    return candidate;
            }

            return null;
        }

        private static VLATSettings CreateProjectSettingsAsset(VLATSettings seed = null)
        {
            EnsureProjectFolders();

            VLATSettings existing = AssetDatabase.LoadAssetAtPath<VLATSettings>(ProjectSettingsAssetPath);
            if (existing != null)
                return existing;

            VLATSettings newSettings = ScriptableObject.CreateInstance<VLATSettings>();

            if (seed != null)
            {
                EditorUtility.CopySerialized(seed, newSettings);
                newSettings.name = VLATSettings.ResourceName;
            }

            AssetDatabase.CreateAsset(newSettings, ProjectSettingsAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"VLAT Settings: Created settings asset at {ProjectSettingsAssetPath}.");
            return AssetDatabase.LoadAssetAtPath<VLATSettings>(ProjectSettingsAssetPath);
        }

        private static void EnsureProjectFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/VERA"))
                AssetDatabase.CreateFolder("Assets", "VERA");

            if (!AssetDatabase.IsValidFolder(ProjectSettingsFolder))
                AssetDatabase.CreateFolder("Assets/VERA", "Resources");
        }


        #endregion
    }
}
#endif
