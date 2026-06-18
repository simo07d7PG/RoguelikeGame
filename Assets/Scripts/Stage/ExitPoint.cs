using UnityEngine;

namespace FinalRogue
{
    [RequireComponent(typeof(Collider2D))]
    public class ExitPoint : MonoBehaviour
    {
        Collider2D col;
        SpriteRenderer spriteRenderer;
        bool isActive;

        void Awake()
        {
            col = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetActive(false);
        }

        public void SetActive(bool active)
        {
            isActive = active;
            if (col != null)
                col.enabled = active;

            if (spriteRenderer != null)
            {
                var color = spriteRenderer.color;
                color.a = active ? 1f : 0.25f;
                spriteRenderer.color = color;
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive)
                return;

            if (other.GetComponent<PlayerEntity>() != null || other.GetComponent<PlayerController>() != null)
                StageManager.Instance?.OnPlayerReachedExit();
        }
    }
}