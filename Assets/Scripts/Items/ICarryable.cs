using UnityEngine;

namespace RoguelikeGame.Items
{
    public interface ICarryable
    {
        Transform Transform { get; }
        bool IsCarried { get; }
        void OnPickedUp(Transform stackSlot);
        void OnDropped(Vector2 worldPosition);
        void OnThrown(Vector2 velocity);
    }
}