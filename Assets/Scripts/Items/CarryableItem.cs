using UnityEngine;

namespace RoguelikeGame.Items
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class CarryableItem : MonoBehaviour, ICarryable
    {
        [SerializeField] private ItemData itemData;

        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private Collider2D[] _colliders;
        private bool _isThrown;
        private Vector2 _throwOrigin;
        private Vector2 _throwDirection;
        private float _throwMaxDistance;

        public Transform Transform => transform;
        public ItemData Data => itemData;
        public bool IsCarried { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = ResolveSpriteRenderer();
            _colliders = GetComponents<Collider2D>();
            ApplyVisualFromData();
        }

        private void OnValidate()
        {
            _spriteRenderer = ResolveSpriteRenderer();
            ApplyVisualFromData();
        }

        private SpriteRenderer ResolveSpriteRenderer()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            return spriteRenderer != null ? spriteRenderer : GetComponentInChildren<SpriteRenderer>();
        }

        private void ApplyVisualFromData()
        {
            if (_spriteRenderer == null || itemData == null || itemData.Icon == null)
            {
                return;
            }

            _spriteRenderer.sprite = itemData.Icon;
        }

        private void FixedUpdate()
        {
            if (!_isThrown)
            {
                return;
            }

            Vector2 offset = (Vector2)transform.position - _throwOrigin;
            float traveled = Vector2.Dot(offset, _throwDirection);

            if (traveled < _throwMaxDistance)
            {
                return;
            }

            transform.position = _throwOrigin + _throwDirection * _throwMaxDistance;
            StopThrownMotion();
        }

        public void ApplyItemData(ItemData data)
        {
            itemData = data;
            ApplyVisualFromData();
        }

        public void OnPlaced(Transform anchor)
        {
            StopThrownMotion();
            IsCarried = false;
            transform.SetParent(anchor, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
            _rigidbody.gravityScale = 0f;
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            SetCollidersEnabled(false);
        }

        public void OnPickedUp(Transform stackSlot)
        {
            StopThrownMotion();
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
            StopThrownMotion();
            ReleaseToWorld(worldPosition, Vector2.zero);
        }

        public void OnThrown(Vector2 velocity, float maxDistance)
        {
            if (velocity.sqrMagnitude < 0.01f || maxDistance <= 0f)
            {
                return;
            }

            ReleaseToWorld(transform.position, velocity);

            _isThrown = true;
            _throwOrigin = transform.position;
            _throwDirection = velocity.normalized;
            _throwMaxDistance = maxDistance;
        }

        private void ReleaseToWorld(Vector2 worldPosition, Vector2 velocity)
        {
            IsCarried = false;
            transform.SetParent(null, true);
            transform.position = worldPosition;

            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.gravityScale = 0f;
            _rigidbody.linearVelocity = velocity;
            _rigidbody.angularVelocity = 0f;
            SetCollidersEnabled(true);
        }

        private void StopThrownMotion()
        {
            _isThrown = false;
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
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