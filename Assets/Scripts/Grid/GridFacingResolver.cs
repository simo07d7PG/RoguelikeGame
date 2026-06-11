using UnityEngine;

namespace RoguelikeGame.Grid
{
    public class GridFacingResolver : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Grid unityGrid;
        [SerializeField] private float facingCellDistance = 0.5f;

        public UnityEngine.Grid UnityGrid => unityGrid;

        public bool TryGetFacingCell(Vector2 worldPosition, Vector2 facingDirection, out GridCoordinate cell)
        {
            cell = default;

            if (unityGrid == null || facingDirection.sqrMagnitude < 0.01f)
            {
                return false;
            }

            Vector2 samplePosition = worldPosition + facingDirection.normalized * facingCellDistance;
            Vector3Int gridCell = unityGrid.WorldToCell(samplePosition);
            cell = new GridCoordinate(gridCell);
            return true;
        }

        public Vector3 GetCellCenterWorld(GridCoordinate cell)
        {
            return unityGrid != null
                ? unityGrid.GetCellCenterWorld(cell.ToVector3Int())
                : Vector3.zero;
        }

        public Vector3 GetCellCenterWorld(Vector3Int cell)
        {
            return unityGrid != null ? unityGrid.GetCellCenterWorld(cell) : Vector3.zero;
        }
    }
}