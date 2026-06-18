using UnityEngine;

namespace FinalRogue
{
    public static class UIHierarchyUtility
    {
        public static GameObject FindObject(Transform root, string objectName)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == objectName)
                    return child.gameObject;
            }

            return null;
        }

        public static T FindComponent<T>(Transform root, string objectName) where T : Component
        {
            GameObject target = FindObject(root, objectName);
            return target != null ? target.GetComponent<T>() : null;
        }
    }
}