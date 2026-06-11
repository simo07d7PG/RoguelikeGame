using UnityEngine;

namespace RoguelikeGame.Utilities
{
    public static class Direction8
    {
        public static readonly Vector2 Up = Vector2.up;
        public static readonly Vector2 Down = Vector2.down;
        public static readonly Vector2 Left = Vector2.left;
        public static readonly Vector2 Right = Vector2.right;
        public static readonly Vector2 UpRight = new Vector2(1f, 1f).normalized;
        public static readonly Vector2 UpLeft = new Vector2(-1f, 1f).normalized;
        public static readonly Vector2 DownRight = new Vector2(1f, -1f).normalized;
        public static readonly Vector2 DownLeft = new Vector2(-1f, -1f).normalized;

        public static Vector2 Snap(Vector2 input)
        {
            if (input.sqrMagnitude < 0.01f)
            {
                return Vector2.zero;
            }

            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            if (angle < 0f)
            {
                angle += 360f;
            }

            if (angle >= 337.5f || angle < 22.5f)
            {
                return Right;
            }

            if (angle < 67.5f)
            {
                return UpRight;
            }

            if (angle < 112.5f)
            {
                return Up;
            }

            if (angle < 157.5f)
            {
                return UpLeft;
            }

            if (angle < 202.5f)
            {
                return Left;
            }

            if (angle < 247.5f)
            {
                return DownLeft;
            }

            if (angle < 292.5f)
            {
                return Down;
            }

            return DownRight;
        }

        public static bool IsDiagonal(Vector2 direction)
        {
            return Mathf.Abs(direction.x) > 0.01f && Mathf.Abs(direction.y) > 0.01f;
        }
    }
}