using UnityEngine;
using UnityEngine.InputSystem;

namespace FinalRogue
{
    public class GameInput : MonoBehaviour
    {
        [SerializeField] InputActionAsset inputActions;

        InputActionMap playerMap;
        InputAction moveAction;
        InputAction attackAction;
        InputAction reloadAction;

        public Vector2 Move => moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        public bool AttackHeld => attackAction?.IsPressed() ?? false;
        public bool ReloadPressed => reloadAction?.WasPressedThisFrame() ?? false;

        void Awake() => InitializeActions();

        public void AssignInputActions(InputActionAsset asset)
        {
            inputActions = asset;
            InitializeActions();
        }

        void InitializeActions()
        {
            if (inputActions != null)
            {
                playerMap = inputActions.FindActionMap("Player");
                moveAction = playerMap.FindAction("Move");
                attackAction = playerMap.FindAction("Attack");
            }

            reloadAction?.Dispose();
            reloadAction = new InputAction("Reload", InputActionType.Button, "<Keyboard>/r");
        }

        void OnEnable() => EnsureEnabled();

        public void EnsureEnabled()
        {
            if (playerMap == null)
                InitializeActions();

            playerMap?.Enable();
            reloadAction?.Enable();
        }

        void OnDisable()
        {
            reloadAction?.Disable();
            playerMap?.Disable();
        }

        void OnDestroy() => reloadAction?.Dispose();

        public Vector2 GetMouseScreenPosition()
        {
            if (Mouse.current == null)
                return Vector2.zero;

            return Mouse.current.position.ReadValue();
        }
    }
}