using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FinalRogue
{
    public static class EntitySetupUtility
    {
        public const string DefaultLobbySceneName = "LobbyScene";
        public const string DefaultGameSceneName = "MainScene";
        public const string DefaultInputActionsPath = "Assets/InputSystem_Actions.inputactions";
        public const string DefaultStartingWeaponPath = "Assets/SO/HG.asset";
        public const string DefaultShopCatalogPath = "Assets/SO/ShopCatalog.asset";
        public const string DefaultEnemyPrefabPath = "Assets/Prefabs/Enemy.prefab";
        public const string DefaultEnemyDataPath = "Assets/SO/EnemyData.asset";

        public static T EnsureComponent<T>(GameObject gameObject) where T : Component
        {
            if (!gameObject.TryGetComponent(out T component))
                component = gameObject.AddComponent<T>();
            return component;
        }

        public static Transform EnsureChild(Transform parent, string childName)
        {
            Transform child = parent.Find(childName);
            if (child != null)
                return child;

            var childObject = new GameObject(childName);
            childObject.transform.SetParent(parent, false);
            return childObject.transform;
        }

        public static void ConfigureRigidbody2D(Rigidbody2D rb)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public static void ConfigureCircleCollider2D(CircleCollider2D collider, Vector2 offset, float radius)
        {
            collider.isTrigger = false;
            collider.offset = offset;
            collider.radius = radius;
        }

        public static void SetField(Component component, string fieldName, Object value)
        {
#if UNITY_EDITOR
            if (component == null)
                return;

            SerializedObject serializedObject = new(component);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            if (property != null)
            {
                property.objectReferenceValue = value;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
#endif
        }

        public static void SetField(Component component, string fieldName, float value)
        {
#if UNITY_EDITOR
            if (component == null)
                return;

            SerializedObject serializedObject = new(component);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            if (property != null)
            {
                property.floatValue = value;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
#endif
        }

        public static void SetField(Component component, string fieldName, int value)
        {
#if UNITY_EDITOR
            if (component == null)
                return;

            SerializedObject serializedObject = new(component);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            if (property != null)
            {
                property.intValue = value;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
#endif
        }

        public static T FindFirst<T>() where T : Object
        {
            return Object.FindFirstObjectByType<T>(FindObjectsInactive.Exclude);
        }

        public static T[] FindAll<T>(FindObjectsSortMode sortMode = FindObjectsSortMode.None) where T : Object
        {
            return Object.FindObjectsByType<T>(FindObjectsInactive.Include, sortMode);
        }

#if UNITY_EDITOR
        public static T LoadAssetAtPath<T>(string path) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public static void SetFieldArray(Component component, string fieldName, Object[] values)
        {
            if (component == null || values == null)
                return;

            SerializedObject serializedObject = new(component);
            SerializedProperty property = serializedObject.FindProperty(fieldName);
            if (property == null || !property.isArray)
                return;

            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
#endif
    }
}