using RoguelikeGame.Cooking;
using UnityEngine;

namespace RoguelikeGame.Stations
{
    public class StationVisualFeedback : MonoBehaviour
    {
        [SerializeField] private KitchenStationBase station;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color emptyColor = Color.white;
        [SerializeField] private Color processingColor = new Color(1f, 0.9f, 0.4f);
        [SerializeField] private Color readyColor = new Color(0.4f, 1f, 0.5f);
        [SerializeField] private Color burnedColor = new Color(0.35f, 0.35f, 0.35f);
        [SerializeField] private Color fireColor = new Color(1f, 0.3f, 0.1f);

        private void Awake()
        {
            if (station == null)
            {
                station = GetComponent<KitchenStationBase>();
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (station != null)
            {
                station.StateChanged += HandleStateChanged;
                HandleStateChanged(station.CurrentState);
            }
        }

        private void OnDestroy()
        {
            if (station != null)
            {
                station.StateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged(StationState state)
        {
            if (spriteRenderer == null)
            {
                return;
            }

            spriteRenderer.color = state switch
            {
                StationState.Processing => processingColor,
                StationState.Ready => readyColor,
                StationState.Burned => burnedColor,
                StationState.OnFire => fireColor,
                _ => emptyColor
            };
        }
    }
}