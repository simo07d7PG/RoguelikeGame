using System.Collections.Generic;
using RoguelikeGame.Core;
using RoguelikeGame.Items;
using UnityEngine;

namespace RoguelikeGame.Player
{
    public class PlayerItemStack : MonoBehaviour
    {
        [SerializeField] private int maxStack = GameConstants.DefaultMaxStack;
        [SerializeField] private Transform[] slotTransforms;

        private readonly List<ICarryable> _items = new List<ICarryable>();

        public int Count => _items.Count;
        public bool IsEmpty => _items.Count == 0;
        public bool IsFull => _items.Count >= maxStack;

        public bool TryPush(ICarryable item)
        {
            if (item == null || IsFull || item.IsCarried)
            {
                return false;
            }

            _items.Add(item);
            RefreshVisuals();
            return true;
        }

        public ICarryable TryPop()
        {
            if (IsEmpty)
            {
                return null;
            }

            ICarryable item = _items[_items.Count - 1];
            _items.RemoveAt(_items.Count - 1);
            RefreshVisuals();
            return item;
        }

        public ICarryable Peek()
        {
            return IsEmpty ? null : _items[_items.Count - 1];
        }

        public void RotateStack()
        {
            if (_items.Count < 2)
            {
                return;
            }

            ICarryable topItem = _items[_items.Count - 1];
            _items.RemoveAt(_items.Count - 1);
            _items.Insert(0, topItem);
            RefreshVisuals();
        }

        private void RefreshVisuals()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                Transform slot = GetSlotTransform(i);
                _items[i].OnPickedUp(slot);
            }
        }

        private Transform GetSlotTransform(int index)
        {
            if (slotTransforms != null && index < slotTransforms.Length && slotTransforms[index] != null)
            {
                return slotTransforms[index];
            }

            return transform;
        }
    }
}