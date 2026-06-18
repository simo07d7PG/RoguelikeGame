using UnityEngine;
using UnityEngine.InputSystem;

namespace FinalRogue
{
    [DefaultExecutionOrder(-200)]
    [AddComponentMenu("FinalRogue/Player Entity")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(GameInput))]
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(PlayerAim))]
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(WeaponController))]
    public class PlayerEntity : MonoBehaviour
    {
        [SerializeField] InputActionAsset inputActions;
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] int maxHealth = 100;

        void Reset() => SetupEntity();
        void Awake() => SetupEntity();

        void SetupEntity()
        {
            Rigidbody2D rb = EntitySetupUtility.EnsureComponent<Rigidbody2D>(gameObject);
            CircleCollider2D collider = EntitySetupUtility.EnsureComponent<CircleCollider2D>(gameObject);
            EntitySetupUtility.EnsureComponent<SpriteRenderer>(gameObject);

            GameInput gameInput = EntitySetupUtility.EnsureComponent<GameInput>(gameObject);
            PlayerController movement = EntitySetupUtility.EnsureComponent<PlayerController>(gameObject);
            PlayerAim aim = EntitySetupUtility.EnsureComponent<PlayerAim>(gameObject);
            EntitySetupUtility.EnsureComponent<PlayerHealth>(gameObject);
            WeaponController weapon = EntitySetupUtility.EnsureComponent<WeaponController>(gameObject);

            EntitySetupUtility.ConfigureRigidbody2D(rb);
            EntitySetupUtility.ConfigureCircleCollider2D(collider, new Vector2(0f, 0.5f), 0.5f);

            Transform firePoint = EntitySetupUtility.EnsureChild(transform, "FirePoint");
            firePoint.localPosition = new Vector3(0f, 0.5f, 0f);

            if (inputActions == null)
                inputActions = LoadDefaultInputActions();

            EntitySetupUtility.SetField(this, "inputActions", inputActions);
            gameInput.AssignInputActions(inputActions);

            movement.Configure(gameInput, moveSpeed);
            aim.Configure(gameInput, transform, firePoint, null);
            weapon.Configure(firePoint, gameInput, aim);
            GetComponent<PlayerHealth>().Configure(maxHealth);
        }

        static InputActionAsset LoadDefaultInputActions()
        {
#if UNITY_EDITOR
            return EntitySetupUtility.LoadAssetAtPath<InputActionAsset>(EntitySetupUtility.DefaultInputActionsPath);
#else
            return null;
#endif
        }
    }
}