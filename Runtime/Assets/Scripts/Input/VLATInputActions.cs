using UnityEngine;
using UnityEngine.InputSystem;

namespace VLAT
{
    /// <summary>
    /// Provides shared access to VLAT's InputActionAsset.
    /// </summary>
    public static class VLATInputActions
    {
        private const string ResourcePath = "VLAT_InputActions";

        private static InputActionAsset _asset;

        public static InputActionAsset Asset
        {
            get
            {
                if (_asset != null)
                    return _asset;

                _asset = Resources.Load<InputActionAsset>(ResourcePath);
                if (_asset != null)
                    return _asset;

                foreach (InputActionAsset asset in Resources.FindObjectsOfTypeAll<InputActionAsset>())
                {
                    if (asset.name == "VLAT_InputActions" || asset.name == "InputActions")
                    {
                        _asset = asset;
                        return _asset;
                    }
                }

                Debug.LogError($"VLATInputActions: Could not load InputActionAsset from Resources/{ResourcePath}.");
                return null;
            }
        }
    }
}
