using RoguelikeGame.Cooking;
using UnityEngine;

namespace RoguelikeGame.Items
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "RoguelikeGame/Item Data")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [SerializeField] [TextArea] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private ItemProcessState processState = ItemProcessState.Raw;
        [SerializeField] private ItemData choppedResult;
        [SerializeField] private ItemData cookedResult;
        [SerializeField] private ItemData burnedResult;
        [SerializeField] private float processDurationOverride;

        public string ItemId => itemId;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public ItemProcessState ProcessState => processState;
        public float ProcessDurationOverride => processDurationOverride;

        public ItemData GetProcessedResult(StationType stationType)
        {
            switch (stationType)
            {
                case StationType.CuttingBoard:
                    return choppedResult;
                case StationType.Stove:
                    return cookedResult;
                default:
                    return null;
            }
        }

        public ItemData GetBurnedResult()
        {
            return burnedResult;
        }

        public float ResolveProcessDuration(float stationDefaultDuration)
        {
            return processDurationOverride > 0f ? processDurationOverride : stationDefaultDuration;
        }
    }
}