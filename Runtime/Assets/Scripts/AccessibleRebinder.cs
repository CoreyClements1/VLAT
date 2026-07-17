using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using VERA;

namespace VLAT
{
    [Preserve]
    public class AccessibleRebinder : MonoBehaviour
    {
        private static readonly string[] SwitchActionNames = { "Switch1", "Switch2", "Switch3", "Switch4" };

        /// <summary>
        /// Active switch-input count from the last applied accessibility settings (2 or 4).
        /// </summary>
        public static int ActiveSwitchInputCount { get; private set; } = 4;

        /// <summary>
        /// True when the active scheme is 2-input (next + select only).
        /// </summary>
        public static bool UsesTwoInputControls => ActiveSwitchInputCount == 2;

        private bool subscribedToVera;

        private void OnEnable()
        {
            StartCoroutine(SubscribeWhenVeraReady());
        }

        private void OnDisable()
        {
            if (!subscribedToVera || !VERASessionManager.IsReady)
                return;

            VERASessionManager.onInitialized.RemoveListener(OnVeraInitialized);
            subscribedToVera = false;
        }

        private IEnumerator SubscribeWhenVeraReady()
        {
            while (!VERASessionManager.IsReady)
                yield return null;

            if (subscribedToVera)
                yield break;

            VERASessionManager.onInitialized.AddListener(OnVeraInitialized);
            subscribedToVera = true;

            if (VERASessionManager.initialized)
                OnVeraInitialized();
        }

        private void OnVeraInitialized()
        {
            VERASessionManager.FetchAccessibilitySettings(ApplyAccessibilitySettings, OnFetchFailed);
        }

        private void OnFetchFailed(string error)
        {
            Debug.LogWarning($"AccessibleRebinder: Could not load accessibility settings: {error}");

            if (ShouldForceEnableAccessibleControls())
            {
                Debug.Log("[VLAT] Forcing accessible controls with default settings (testing mode).");
                ApplyDefaultAccessibilitySettings();
            }
        }

        private void ApplyAccessibilitySettings(VERAAccessibilitySettings settings)
        {
            if (settings != null && settings.useSwitchControls && settings.switchBindings != null)
            {
                Debug.Log("[VLAT] Participant settings are set to apply accessibility settings. Applying now.");
                StartCoroutine(ApplyAccessibilitySettingsCoroutine(settings));
                return;
            }

            if (ShouldForceEnableAccessibleControls())
            {
                Debug.Log("[VLAT] Participant settings unavailable or disabled; forcing accessible controls with defaults (testing mode).");
                ApplyDefaultAccessibilitySettings();
                return;
            }

            Debug.Log("[VLAT] Participant settings are not set to apply accessibility settings. Skipping.");
        }

        private void ApplyDefaultAccessibilitySettings()
        {
            VERAAccessibilitySettings defaults = new VERAAccessibilitySettings
            {
                useSwitchControls = true,
                switchInputCount = 4,
                // Empty binding set keeps the Input Actions asset defaults.
                switchBindings = new VERASwitchBindingSet()
            };

            StartCoroutine(ApplyAccessibilitySettingsCoroutine(defaults));
        }

        private static bool ShouldForceEnableAccessibleControls()
        {
            VLATSettings settings = VLATSettings.GetOrLoad();
            return settings != null && settings.alwaysEnableAccessibleControlsForTesting;
        }

        private IEnumerator ApplyAccessibilitySettingsCoroutine(VERAAccessibilitySettings settings)
        {
            VLATMenuSpawner.EnsureMenuPresent();
            yield return null;

            if (settings.switchBindings == null)
                yield break;

            bool twoInput = settings.UsesTwoInputControls;
            ActiveSwitchInputCount = twoInput ? 2 : 4;

            Debug.Log($"[VLAT] Applying {(twoInput ? "2-input" : "4-input")} switch control scheme.");

            if (twoInput)
            {
                // Next + select only; previous/back are UI-driven.
                ApplySwitchBinding(SwitchActionNames[1], settings.switchBindings.next);
                ApplySwitchBinding(SwitchActionNames[2], settings.switchBindings.select);
                SetSwitchActionEnabled(SwitchActionNames[0], false);
                SetSwitchActionEnabled(SwitchActionNames[1], true);
                SetSwitchActionEnabled(SwitchActionNames[2], true);
                SetSwitchActionEnabled(SwitchActionNames[3], false);
            }
            else
            {
                ApplySwitchBinding(SwitchActionNames[0], settings.switchBindings.previous);
                ApplySwitchBinding(SwitchActionNames[1], settings.switchBindings.next);
                ApplySwitchBinding(SwitchActionNames[2], settings.switchBindings.select);
                ApplySwitchBinding(SwitchActionNames[3], settings.switchBindings.back);
                SetSwitchActionEnabled(SwitchActionNames[0], true);
                SetSwitchActionEnabled(SwitchActionNames[1], true);
                SetSwitchActionEnabled(SwitchActionNames[2], true);
                SetSwitchActionEnabled(SwitchActionNames[3], true);
            }

            NewMenuNavigation menuNavigation = VLATMenuSpawner.FindMenuNavigation();
            if (menuNavigation != null)
                menuNavigation.ApplyInputSchemeVisuals(twoInput);

            if (twoInput)
            {
                UpdateBindingDisplay(1, settings.switchBindings.next, SwitchActionNames[1]);
                UpdateBindingDisplay(2, settings.switchBindings.select, SwitchActionNames[2]);
            }
            else
            {
                UpdateBindingDisplay(0, settings.switchBindings.previous, SwitchActionNames[0]);
                UpdateBindingDisplay(1, settings.switchBindings.next, SwitchActionNames[1]);
                UpdateBindingDisplay(2, settings.switchBindings.select, SwitchActionNames[2]);
                UpdateBindingDisplay(3, settings.switchBindings.back, SwitchActionNames[3]);
            }
        }

        private void ApplySwitchBinding(string actionName, VERASwitchBinding binding)
        {
            if (binding == null || string.IsNullOrEmpty(binding.bindingPath))
                return;

            InputAction action = FindSwitchAction(actionName);
            if (action == null)
                return;

            bool wasEnabled = action.enabled;
            if (wasEnabled)
                action.Disable();

            action.RemoveAllBindingOverrides();

            // Each Switch action ships with multiple defaults (keyboard, gamepad, XR).
            // Only the rebound path should remain active — empty-path overrides silence
            // the rest so Oculus X/Y/A/B (etc.) do not keep working alongside the rebind.
            if (action.bindings.Count == 0)
            {
                action.AddBinding(binding.bindingPath);
            }
            else
            {
                action.ApplyBindingOverride(0, binding.bindingPath);
                for (int i = 1; i < action.bindings.Count; i++)
                    action.ApplyBindingOverride(i, string.Empty);
            }

            if (wasEnabled)
                action.Enable();
        }

        private static void SetSwitchActionEnabled(string actionName, bool enabled)
        {
            InputAction action = FindSwitchAction(actionName);
            if (action == null)
                return;

            if (enabled)
            {
                if (!action.enabled)
                    action.Enable();
            }
            else if (action.enabled)
            {
                action.Disable();
            }
        }

        private static InputAction FindSwitchAction(string actionName)
        {
            InputActionAsset inputActionAsset = VLATInputActions.Asset;
            if (inputActionAsset == null)
                return null;

            InputAction action = inputActionAsset.FindAction(actionName);
            if (action == null)
                Debug.LogWarning($"AccessibleRebinder: Could not find input action \"{actionName}\".");

            return action;
        }

        private static void UpdateBindingDisplay(int displayIndex, VERASwitchBinding binding, string actionName)
        {
            NewMenuNavigation menuNavigation = VLATMenuSpawner.FindMenuNavigation();
            if (menuNavigation == null)
                return;

            string raw = null;
            if (binding != null)
            {
                if (!string.IsNullOrEmpty(binding.label))
                    raw = binding.label;
                else if (!string.IsNullOrEmpty(binding.bindingPath))
                    raw = binding.bindingPath;
            }

            string displayName = !string.IsNullOrEmpty(raw)
                ? BindingDisplayFormatter.Format(raw)
                : BindingDisplayFormatter.FromAction(FindSwitchAction(actionName));

            if (string.IsNullOrEmpty(displayName))
                return;

            menuNavigation.ResetBindingDisplayName(displayIndex, displayName);
        }
    }
}
