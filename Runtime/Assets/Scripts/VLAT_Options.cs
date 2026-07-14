using UnityEngine;

namespace VLAT
{
    /// <summary>
    /// Runtime applicator for VLAT settings. Configuration is edited globally via
    /// menu bar → VERA → VLAT Settings (see <see cref="VLATSettings"/>).
    /// </summary>
    public class VLAT_Options : MonoBehaviour
    {
        public static VLAT_Options Instance;

        private VLATSettings settings;
        private GameObject xrPlayerParent;
        private GameObject leftVirtualHand;
        private GameObject rightVirtualHand;
        private NewMenuNavigation menuNav;
        private bool skipSetupOnStart;


        #region SETUP


        /// <summary>
        /// Prepares a runtime-spawned VLAT_Menu instance before its first frame update.
        /// </summary>
        public void PrepareForRuntimeSpawn(bool skipAccessibilitySetup = false)
        {
            skipSetupOnStart = skipAccessibilitySetup;
            ResolveXRReferencesIfNeeded();
        }


        void Start()
        {
            SetupSingleton();
            if (Instance != this)
                return;

            settings = VLATSettings.GetOrLoad();
            ResolveXRReferencesIfNeeded();
            SetupMovement();
            SetupInteraction();
            SetupSettings();
            SetupMenu();
        }


        private void SetupSingleton()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }


        private void SetupMovement()
        {
            if (xrPlayerParent == null)
            {
                Debug.LogError("No XR player parent could be resolved for VLAT options. Ensure an XR Origin exists in the scene.");
                return;
            }

#if UNITY_2023_1_OR_NEWER
            MovementController moveControl = FindAnyObjectByType<MovementController>();
#else
            MovementController moveControl = FindObjectOfType<MovementController>();
#endif

            if (moveControl != null)
                moveControl.Setup(xrPlayerParent, settings.xrPlayerRadius, settings.xrPlayerHeight, settings.playerMoveSpeed);
            else
                Debug.LogError("No VLAT MovementController could be found in scene; VLAT movement will not work.");
        }


        private void SetupInteraction()
        {
#if UNITY_2023_1_OR_NEWER
            SelectionController selectControl = FindAnyObjectByType<SelectionController>();
#else
            SelectionController selectControl = FindObjectOfType<SelectionController>();
#endif

            if (selectControl != null)
                selectControl.Setup(settings.interactionRadius);
            else
                Debug.LogError("No VLAT SelectionController could be found in scene; VLAT interaction will not work.");
        }


        private void SetupSettings()
        {
#if UNITY_2023_1_OR_NEWER
            SettingsManager settingsControl = FindAnyObjectByType<SettingsManager>();
#else
            SettingsManager settingsControl = FindObjectOfType<SettingsManager>();
#endif

            if (settingsControl != null)
                settingsControl.Setup(settings.playerHasVirtualHands, leftVirtualHand, rightVirtualHand);
            else
                Debug.LogError("No VLAT SettingsManager could be found in scene; VLAT settings functionality will not work.");
        }


        private void ResolveXRReferencesIfNeeded()
        {
            if (xrPlayerParent != null && leftVirtualHand != null && rightVirtualHand != null)
                return;

            if (!VLATXRReferences.TryResolve(out GameObject resolvedPlayerParent, out GameObject resolvedLeftHand, out GameObject resolvedRightHand))
                return;

            if (xrPlayerParent == null)
                xrPlayerParent = resolvedPlayerParent;
            if (leftVirtualHand == null)
                leftVirtualHand = resolvedLeftHand;
            if (rightVirtualHand == null)
                rightVirtualHand = resolvedRightHand;
        }


        private void SetupMenu()
        {
#if UNITY_2023_1_OR_NEWER
            menuNav = FindAnyObjectByType<NewMenuNavigation>();
#else
            menuNav = FindObjectOfType<NewMenuNavigation>();
#endif
            if (menuNav == null)
            {
                Debug.LogError("No VLAT NewMenuNavigation could be found in scene; VLAT menu will not work.");
                return;
            }

            menuNav.Setup();

            if (settings.showSetupOnStart && !skipSetupOnStart)
                ShowSetupOptions();
        }


        #endregion


        #region ON / OFF


        public void HideVlatMenu()
        {
#if UNITY_2023_1_OR_NEWER
            FindAnyObjectByType<NewMenuNavigation>().HideVlatMenu();
#else
            FindObjectOfType<NewMenuNavigation>().HideVlatMenu();
#endif
        }


        public void ShowVlatMenu()
        {
#if UNITY_2023_1_OR_NEWER
            FindAnyObjectByType<NewMenuNavigation>().ShowVlatMenu();
#else
            FindObjectOfType<NewMenuNavigation>().ShowVlatMenu();
#endif
        }


        #endregion


        #region OTHER


        public GameObject GetXrPlayerParent()
        {
            return xrPlayerParent;
        }


        public void ShowSetupOptions()
        {
            menuNav.ShowSetupOptions();
        }


        #endregion
    }
}
