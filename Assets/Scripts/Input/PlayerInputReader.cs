using RoguelikeGame.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RoguelikeGame.Input
{
    public class PlayerInputReader : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool ThrowHeld { get; private set; }
        public bool ThrowReleased { get; private set; }
        public bool StackRotatePressed { get; private set; }

        public void Refresh()
        {
            MoveInput = ReadMoveInput();
            InteractPressed = ReadInteractPressed();
            ThrowHeld = ReadThrowHeld();
            ThrowReleased = ReadThrowReleased();
            StackRotatePressed = ReadStackRotatePressed();
        }

        private static Vector2 ReadMoveInput()
        {
            Vector2 move = new Vector2(
                UnityEngine.Input.GetAxisRaw("Horizontal"),
                UnityEngine.Input.GetAxisRaw("Vertical"));

            return Direction8.Snap(move);
        }

        private static bool ReadInteractPressed()
        {
            bool keyboardPressed = Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame;
            bool mousePressed = Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
            return keyboardPressed || mousePressed;
        }

        private static bool ReadThrowHeld()
        {
            bool keyboardHeld = Keyboard.current != null && Keyboard.current.qKey.isPressed;
            bool mouseHeld = Mouse.current != null && Mouse.current.leftButton.isPressed;
            return keyboardHeld || mouseHeld;
        }

        private static bool ReadThrowReleased()
        {
            bool keyboardReleased = Keyboard.current != null && Keyboard.current.qKey.wasReleasedThisFrame;
            bool mouseReleased = Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;
            return keyboardReleased || mouseReleased;
        }

        private static bool ReadStackRotatePressed()
        {
            if (Keyboard.current == null)
            {
                return false;
            }

            return Keyboard.current.leftShiftKey.wasPressedThisFrame
                || Keyboard.current.rightShiftKey.wasPressedThisFrame;
        }
    }
}