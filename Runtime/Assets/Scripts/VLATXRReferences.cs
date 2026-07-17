using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

namespace VLAT
{
    /// <summary>
    /// Resolves standard XRI rig references used by VLAT_Options.
    /// </summary>
    public static class VLATXRReferences
    {
        public static bool TryResolve(out GameObject xrPlayerParent, out GameObject leftVirtualHand, out GameObject rightVirtualHand)
        {
            xrPlayerParent = null;
            leftVirtualHand = null;
            rightVirtualHand = null;

            Transform leftController = null;
            Transform rightController = null;

            XROrigin xrOrigin = FindXROrigin();
            if (xrOrigin != null)
                xrPlayerParent = xrOrigin.gameObject;

            XRController[] controllers = FindControllers();
            foreach (XRController controller in controllers)
            {
                if (controller.controllerNode == XRNode.LeftHand && leftController == null)
                    leftController = controller.transform;
                else if (controller.controllerNode == XRNode.RightHand && rightController == null)
                    rightController = controller.transform;
            }

            if (xrOrigin != null && (leftController == null || rightController == null))
            {
                foreach (XRController controller in xrOrigin.GetComponentsInChildren<XRController>(true))
                {
                    if (controller.controllerNode == XRNode.LeftHand && leftController == null)
                        leftController = controller.transform;
                    else if (controller.controllerNode == XRNode.RightHand && rightController == null)
                        rightController = controller.transform;
                }
            }

            if (leftController == null || rightController == null)
                FindActionBasedControllers(ref leftController, ref rightController);

            if (leftController == null || rightController == null)
                FindControllersByName(xrOrigin, ref leftController, ref rightController);

            if (leftController != null)
                leftVirtualHand = leftController.gameObject;
            if (rightController != null)
                rightVirtualHand = rightController.gameObject;

            return xrPlayerParent != null;
        }

        public static Camera FindMainXRCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
                return mainCamera;

            XROrigin xrOrigin = FindXROrigin();
            if (xrOrigin != null && xrOrigin.Camera != null)
                return xrOrigin.Camera;

#if UNITY_2023_1_OR_NEWER
            return Object.FindAnyObjectByType<Camera>();
#else
            return Object.FindObjectOfType<Camera>();
#endif
        }

        private static XROrigin FindXROrigin()
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindAnyObjectByType<XROrigin>();
#else
            return Object.FindObjectOfType<XROrigin>();
#endif
        }

        private static XRController[] FindControllers()
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindObjectsByType<XRController>(FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType<XRController>();
#endif
        }

        private static void FindActionBasedControllers(ref Transform leftController, ref Transform rightController)
        {
#if UNITY_2023_1_OR_NEWER
            ActionBasedController[] controllers = Object.FindObjectsByType<ActionBasedController>(FindObjectsSortMode.None);
#else
            ActionBasedController[] controllers = Object.FindObjectsOfType<ActionBasedController>();
#endif
            foreach (ActionBasedController controller in controllers)
            {
                string hand = GetHandFromAction(controller.positionAction);
                if (hand == "left" && leftController == null)
                    leftController = controller.transform;
                else if (hand == "right" && rightController == null)
                    rightController = controller.transform;
            }
        }

        private static string GetHandFromAction(UnityEngine.InputSystem.InputActionProperty actionProperty)
        {
            string path = actionProperty.action?.bindings.Count > 0
                ? actionProperty.action.bindings[0].path
                : actionProperty.reference?.action?.bindings.Count > 0
                    ? actionProperty.reference.action.bindings[0].path
                    : null;

            if (string.IsNullOrEmpty(path))
                return null;

            string lowerPath = path.ToLower();
            if (lowerPath.Contains("lefthand") || lowerPath.Contains("left hand"))
                return "left";
            if (lowerPath.Contains("righthand") || lowerPath.Contains("right hand"))
                return "right";

            return null;
        }

        private static void FindControllersByName(XROrigin xrOrigin, ref Transform leftController, ref Transform rightController)
        {
            Transform searchRoot = xrOrigin != null ? xrOrigin.transform : null;

            if (searchRoot != null)
            {
                foreach (Transform transform in searchRoot.GetComponentsInChildren<Transform>(true))
                {
                    TryAssignControllerByName(transform, ref leftController, ref rightController);
                }
            }

            if (leftController != null && rightController != null)
                return;

#if UNITY_2023_1_OR_NEWER
            Transform[] allTransforms = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
#else
            Transform[] allTransforms = Object.FindObjectsOfType<Transform>();
#endif
            foreach (Transform transform in allTransforms)
                TryAssignControllerByName(transform, ref leftController, ref rightController);
        }

        private static void TryAssignControllerByName(Transform transform, ref Transform leftController, ref Transform rightController)
        {
            string name = transform.name.ToLower();

            if (leftController == null &&
                name.Contains("left") &&
                (name.Contains("controller") || name.Contains("hand") || name.Contains("grip")))
            {
                leftController = transform;
                return;
            }

            if (rightController == null &&
                name.Contains("right") &&
                (name.Contains("controller") || name.Contains("hand") || name.Contains("grip")))
            {
                rightController = transform;
            }
        }
    }
}
