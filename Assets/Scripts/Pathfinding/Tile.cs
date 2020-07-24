using UnityEngine;

namespace Pathfinding
{
    public class Tile
    {
        public const int TILE_SIZE = 1;
        private static readonly Vector3 _HalfExtent = Vector3.one / 2;
        public bool Obstructed { get; private set; }
        public Vector3 Position { get; private set; }

        public float Alpha { get; set; }
        public float Beta { get; set; }
        public Tile Parent { get; set; }
        public float TotalHeuristic => Alpha + Beta;

        public(int x, int z) CollectionIndex { get; private set; }

        public Tile[] Neighbors { get; private set; }

        private Color _gizmosColor;

        public Tile(Vector3 position, int xIndex, int zIndex)
        {
            Position = position;
            CollectionIndex = (xIndex, zIndex);
            UpdateState();
        }

        public void UpdateState()
        {
            Obstructed =
                Physics.CheckBox(
                    Position,
                    _HalfExtent,
                    Quaternion.identity,
                    Grid.ObstaclesMask);

            _gizmosColor = Obstructed ? Color.red : Color.gray;
            _gizmosColor.a = 0.2f;
        }

        public void SetNeighbors(Tile[] neighbors)
        {
            Neighbors = neighbors;
        }

        public float DistanceTo(Vector3 position) =>
            Mathf.RoundToInt(Vector3.Distance(Position, position));

        public bool IsWorseThan(Tile other)
        {
            return other.TotalHeuristic <= TotalHeuristic &&
                other.Alpha < Alpha;
        }

        public void DrawGizmos()
        {
            Gizmos.color = _gizmosColor;
            Gizmos.DrawWireCube(Position, TILE_SIZE * Vector3.one);
        }
    }
}