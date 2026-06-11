using UnityEngine;

namespace RoguelikeGame.Items
{
    public interface ICarryable
    {
        Transform Transform { get; }
        ItemData Data { get; }
        bool IsCarried { get; }
        void ApplyItemData(ItemData data);
        void OnPickedUp(Transform stackSlot);
        void OnPlaced(Transform anchor);
        void OnDropped(Vector2 worldPosition);
        void OnThrown(Vector2 velocity, float maxDistance);
    }
}