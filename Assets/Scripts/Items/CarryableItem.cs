using UnityEngine;

namespace RoguelikeGame.Items
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CarryableItem : MonoBehaviour, ICarryable
    {
        private Rigidbody2D _rigidbody;
        private Collider2D[] _colliders;

        public Transform Transform => transform;
        public bool IsCarried { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _colliders = GetComponents<Collider2D>();
        }

        public void OnPickedUp(Transform stackSlot)
        {
            IsCarried = true;
            transform.SetParent(stackSlot, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
            _rigidbody.gravityScale = 0f;
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            SetCollidersEnabled(false);
        }

        public void OnDropped(Vector2 worldPosition)
        {
            ReleaseToWorld(worldPosition, Vector2.zero, 0f);
        }

        public void OnThrown(Vector2 velocity)
        {
            ReleaseToWorld(transform.position, velocity, 1f);
        }

        private void ReleaseToWorld(Vector2 worldPosition, Vector2 velocity, float gravityScale)
        {
            IsCarried = false;
            transform.SetParent(null, true);
            transform.position = worldPosition;

            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.gravityScale = gravityScale;
            _rigidbody.linearVelocity = velocity;
            _rigidbody.angularVelocity = 0f;
            SetCollidersEnabled(true);
        }

        private void SetCollidersEnabled(bool enabled)
        {
            for (int i = 0; i < _colliders.Length; i++)
            {
                _colliders[i].enabled = enabled;
            }
        }
    }
}