using System;
using UnityEngine;

namespace RoguelikeGame.Grid
{
    [Serializable]
    public struct GridCoordinate : IEquatable<GridCoordinate>
    {
        public int X;
        public int Y;

        public GridCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public GridCoordinate(Vector3Int cell)
        {
            X = cell.x;
            Y = cell.y;
        }

        public Vector3Int ToVector3Int()
        {
            return new Vector3Int(X, Y, 0);
        }

        public bool Equals(GridCoordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is GridCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}