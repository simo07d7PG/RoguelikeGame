using System;
using RoguelikeGame.Items;
using UnityEngine;

namespace RoguelikeGame.Cooking
{
    public class CookingStateMachine
    {
        public StationState CurrentState { get; private set; } = StationState.Empty;
        public float StateTimer { get; private set; }
        public bool IsOnFire => CurrentState == StationState.OnFire;

        public event Action<StationState> StateChanged;
        public event Action FireStarted;

        public void Reset()
        {
            SetState(StationState.Empty);
            StateTimer = 0f;
        }

        public void BeginHolding()
        {
            SetState(StationState.Holding);
            StateTimer = 0f;
        }

        public void BeginProcessing(float duration)
        {
            SetState(StationState.Processing);
            StateTimer = duration;
        }

        public void CompleteProcessing()
        {
            SetState(StationState.Ready);
            StateTimer = 0f;
        }

        public void BeginBurnCountdown(float duration)
        {
            SetState(StationState.Ready);
            StateTimer = duration;
        }

        public void MarkBurned(float fireCountdownDuration)
        {
            SetState(StationState.Burned);
            StateTimer = fireCountdownDuration;
        }

        public void Ignite()
        {
            SetState(StationState.OnFire);
            StateTimer = 0f;
            FireStarted?.Invoke();
        }

        public void Tick(float deltaTime, StationProfile profile, ICarryable heldItem)
        {
            if (profile == null || CurrentState == StationState.Empty || CurrentState == StationState.Holding)
            {
                return;
            }

            if (CurrentState == StationState.Processing)
            {
                StateTimer -= deltaTime;
                if (StateTimer <= 0f)
                {
                    CompleteProcessingItem(heldItem, profile.StationType);
                    if (profile.StationType == StationType.Stove)
                    {
                        BeginBurnCountdown(profile.ReadyToBurnDuration);
                    }
                }

                return;
            }

            if (CurrentState == StationState.Ready && profile.StationType == StationType.Stove)
            {
                StateTimer -= deltaTime;
                if (StateTimer <= 0f)
                {
                    BurnHeldItem(heldItem);
                    MarkBurned(profile.BurnToFireDuration);
                }

                return;
            }

            if (CurrentState == StationState.Burned && profile.StationType == StationType.Stove)
            {
                StateTimer -= deltaTime;
                if (StateTimer <= 0f)
                {
                    Ignite();
                }
            }
        }

        private void CompleteProcessingItem(ICarryable heldItem, StationType stationType)
        {
            if (heldItem?.Data == null)
            {
                CompleteProcessing();
                return;
            }

            ItemData result = heldItem.Data.GetProcessedResult(stationType);
            if (result != null)
            {
                heldItem.ApplyItemData(result);
            }

            CompleteProcessing();
        }

        private static void BurnHeldItem(ICarryable heldItem)
        {
            if (heldItem?.Data == null)
            {
                return;
            }

            ItemData burnedResult = heldItem.Data.GetBurnedResult();
            if (burnedResult != null)
            {
                heldItem.ApplyItemData(burnedResult);
            }
        }

        private void SetState(StationState newState)
        {
            if (CurrentState == newState)
            {
                return;
            }

            CurrentState = newState;
            StateChanged?.Invoke(newState);
        }
    }
}