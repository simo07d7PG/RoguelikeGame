using UnityEngine;

namespace FinalRogue
{
    public class EnemySpawnAreaMarker : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Vector2 markerSize = new(1f, 1f);
        [SerializeField] Color markerColor = new(1f, 0.45f, 0.2f, 0.7f);

        public void Setup(Vector2 size, Color color)
        {
            markerSize = size;
            markerColor = color;
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
            spriteRenderer.color = markerColor;
            spriteRenderer.sortingOrder = 1;
            transform.localScale = new Vector3(markerSize.x, markerSize.y, 1f);
        }
    }
}