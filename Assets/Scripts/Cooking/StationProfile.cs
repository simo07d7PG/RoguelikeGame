using UnityEngine;

namespace RoguelikeGame.Cooking
{
    [CreateAssetMenu(fileName = "StationProfile", menuName = "RoguelikeGame/Station Profile")]
    public class StationProfile : ScriptableObject
    {
        [SerializeField] private StationType stationType = StationType.Counter;
        [SerializeField] private float processDuration = 2f;
        [SerializeField] private float readyToBurnDuration = 5f;
        [SerializeField] private float burnToFireDuration = 3f;
        [SerializeField] private ItemProcessState[] acceptedStates = { ItemProcessState.Raw };

        public StationType StationType => stationType;
        public float ProcessDuration => processDuration;
        public float ReadyToBurnDuration => readyToBurnDuration;
        public float BurnToFireDuration => burnToFireDuration;
        public ItemProcessState[] AcceptedStates => acceptedStates;

        public bool AcceptsState(ItemProcessState state)
        {
            if (acceptedStates == null || acceptedStates.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < acceptedStates.Length; i++)
            {
                if (acceptedStates[i] == state)
                {
                    return true;
                }
            }

            return false;
        }
    }
}