using UnityEngine;

namespace VLAT
{
    public static class VLATMenuSpawner
    {
        private const string MenuResourcePath = "VLAT_Menu";

        public static NewMenuNavigation EnsureMenuPresent()
        {
            NewMenuNavigation existingMenu = FindMenuNavigation();
            if (existingMenu != null)
                return existingMenu;

            GameObject menuPrefab = Resources.Load<GameObject>(MenuResourcePath);
            if (menuPrefab == null)
            {
                Debug.LogError($"VLATMenuSpawner: Could not load menu prefab from Resources/{MenuResourcePath}.");
                return null;
            }

            Camera camera = VLATXRReferences.FindMainXRCamera();
            if (camera == null)
            {
                Debug.LogWarning("VLATMenuSpawner: No main camera found; cannot spawn VLAT_Menu.");
                return null;
            }

            GameObject menuInstance = Object.Instantiate(menuPrefab, camera.transform, false);
            menuInstance.name = "VLAT_Menu";

            VLAT_Options options = menuInstance.GetComponent<VLAT_Options>();
            options?.PrepareForRuntimeSpawn(skipAccessibilitySetup: true);

            return menuInstance.GetComponentInChildren<NewMenuNavigation>(true);
        }

        public static NewMenuNavigation FindMenuNavigation()
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindAnyObjectByType<NewMenuNavigation>();
#else
            return Object.FindObjectOfType<NewMenuNavigation>();
#endif
        }
    }
}
