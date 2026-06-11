using RoguelikeGame.Cooking;
using RoguelikeGame.Grid;
using RoguelikeGame.Interaction;
using RoguelikeGame.Items;
using UnityEngine;

namespace RoguelikeGame.Stations
{
    public abstract class KitchenStationBase : MonoBehaviour, IInteractable
    {
        [SerializeField] private StationProfile stationProfile;
        [SerializeField] private Transform itemAnchor;
        [SerializeField] private UnityEngine.Grid snapGrid;

        private readonly CookingStateMachine _stateMachine = new CookingStateMachine();
        private ICarryable _heldItem;

        public StationProfile Profile => stationProfile;
        public StationState CurrentState => _stateMachine.CurrentState;
        public ICarryable HeldItem => _heldItem;
        public bool IsOnFire => _stateMachine.IsOnFire;

        public event System.Action<StationState> StateChanged;
        public event System.Action FireStarted;

        protected virtual void Awake()
        {
            if (itemAnchor == null)
            {
                itemAnchor = transform;
            }

            SnapToGrid();
            _stateMachine.StateChanged += HandleStateChanged;
            _stateMachine.FireStarted += HandleFireStarted;
        }

        protected virtual void OnDestroy()
        {
            _stateMachine.StateChanged -= HandleStateChanged;
            _stateMachine.FireStarted -= HandleFireStarted;
        }

        protected virtual void Update()
        {
            if (stationProfile == null)
            {
                return;
            }

            _stateMachine.Tick(Time.deltaTime, stationProfile, _heldItem);
        }

        public virtual bool CanPickup(ICarryable item)
        {
            if (_heldItem == null || _stateMachine.IsOnFire || stationProfile == null)
            {
                return false;
            }

            return stationProfile.StationType switch
            {
                StationType.Counter => _stateMachine.CurrentState == StationState.Holding,
                StationType.CuttingBoard => _stateMachine.CurrentState == StationState.Ready,
                StationType.Stove => _stateMachine.CurrentState == StationState.Ready
                    || _stateMachine.CurrentState == StationState.Burned,
                _ => false
            };
        }

        public virtual bool CanPlace(ICarryable item)
        {
            if (item == null || item.Data == null || _heldItem != null || _stateMachine.IsOnFire)
            {
                return false;
            }

            if (stationProfile.StationType == StationType.Counter)
            {
                return true;
            }

            if (!stationProfile.AcceptsState(item.Data.ProcessState))
            {
                return false;
            }

            return item.Data.GetProcessedResult(stationProfile.StationType) != null;
        }

        public virtual void Place(ICarryable item)
        {
            if (!CanPlace(item))
            {
                return;
            }

            _heldItem = item;
            _heldItem.OnPlaced(itemAnchor);

            if (stationProfile.StationType == StationType.Counter)
            {
                _stateMachine.BeginHolding();
                return;
            }

            float duration = item.Data.ResolveProcessDuration(stationProfile.ProcessDuration);
            _stateMachine.BeginProcessing(duration);
        }

        public virtual ICarryable Take()
        {
            if (!CanPickup(null))
            {
                return null;
            }

            ICarryable takenItem = _heldItem;
            _heldItem = null;
            _stateMachine.Reset();
            takenItem.OnDropped(itemAnchor.position);
            return takenItem;
        }

        public bool OccupiesCell(GridCoordinate cell)
        {
            if (snapGrid == null)
            {
                return false;
            }

            Vector3Int stationCell = snapGrid.WorldToCell(transform.position);
            return stationCell.x == cell.X && stationCell.y == cell.Y;
        }

        private void SnapToGrid()
        {
            if (snapGrid == null)
            {
                return;
            }

            Vector3Int cell = snapGrid.WorldToCell(transform.position);
            transform.position = snapGrid.GetCellCenterWorld(cell);
        }

        private void HandleStateChanged(StationState state)
        {
            StateChanged?.Invoke(state);
        }

        private void HandleFireStarted()
        {
            FireStarted?.Invoke();
        }
    }
}