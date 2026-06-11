using RoguelikeGame.Input;
using UnityEngine;

namespace RoguelikeGame.Player
{
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerFacing))]
    [RequireComponent(typeof(PlayerItemStack))]
    [RequireComponent(typeof(PlayerInteraction))]
    [RequireComponent(typeof(PlayerThrow))]
    [RequireComponent(typeof(Interaction.InteractableDetector))]
    public class PlayerController : MonoBehaviour
    {
        private PlayerInputReader _input;
        private PlayerMovement _movement;
        private PlayerFacing _facing;
        private PlayerItemStack _stack;
        private PlayerInteraction _interaction;
        private PlayerThrow _throw;

        private void Awake()
        {
            _input = GetComponent<PlayerInputReader>();
            _movement = GetComponent<PlayerMovement>();
            _facing = GetComponent<PlayerFacing>();
            _stack = GetComponent<PlayerItemStack>();
            _interaction = GetComponent<PlayerInteraction>();
            _throw = GetComponent<PlayerThrow>();
        }

        private void Update()
        {
            _input.Refresh();

            _facing.UpdateFacing(_input.MoveInput);
            _movement.ApplyMovement(_input.MoveInput);

            if (_input.StackRotatePressed)
            {
                _stack.RotateStack();
            }

            if (_input.InteractPressed && !_input.ThrowHeld)
            {
                _interaction.TryInteract();
            }

            _throw.HandleThrowInput(_input.ThrowHeld, _input.ThrowReleased);
        }
    }
}