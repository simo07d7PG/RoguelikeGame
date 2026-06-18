using UnityEngine;

namespace FinalRogue
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Vector2 markerSize = new(1.5f, 1.5f);
        [SerializeField] Color activeColor = new(0.3f, 0.7f, 1f, 0.85f);

        public void Setup(Vector2 size, Color color)
        {
            markerSize = size;
            activeColor = color;
            ApplyVisual();
        }

        void Awake() => EnsureVisual();

        void EnsureVisual()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

            ApplyVisual();
        }

        void ApplyVisual()
        {
            spriteRenderer.sprite = StageMarkerUtility.GetSquareSprite();
            spriteRenderer.color = activeColor;
            spriteRenderer.sortingOrder = 2;
            transform.localScale = new Vector3(markerSize.x, markerSize.y, 1f);
        }
    }
}