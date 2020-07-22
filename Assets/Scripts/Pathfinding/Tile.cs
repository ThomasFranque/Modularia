using UnityEngine;

namespace Pathfinding
{
    public class Tile
    {
        public const int TILE_SIZE = 1;
        private static readonly Vector3 _HalfExtent = Vector3.one / 2;
        public bool Occupied { get; private set; }
        public Vector3 Position { get; private set; }
        private Color _gizmosColor;

        public Tile(Vector3 position)
        {
            Position = position;
            UpdateState();
        }

        public void UpdateState()
        {
            Occupied =
                Physics.CheckBox(
                    Position,
                    _HalfExtent,
                    Quaternion.identity,
                    Grid.ObstaclesMask);

            _gizmosColor = Occupied ? Color.red : Color.gray;
            _gizmosColor.a = 0.2f;
        }

        public float DistanceTo(Vector3 target) =>
            Vector3.Distance(Position, target);

        public void DrawGizmos()
        {
            Gizmos.color = _gizmosColor;
            Gizmos.DrawWireCube(Position, TILE_SIZE * Vector3.one);
        }
    }
}