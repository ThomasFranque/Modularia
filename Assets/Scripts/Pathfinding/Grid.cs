using LevelGeneration.Individuals;
using UnityEngine;

namespace Pathfinding
{
    public class Grid : MonoBehaviour
    {
        public static LayerMask ObstaclesMask { get; private set; }
        private static int RoomSize => Room.ROOM_SIZE;
        private Tile[, ] _tiles;

        public static void SetLayermask(LayerMask l)
        {
            ObstaclesMask = l;
        }

        public void Generate()
        {
            _tiles = new Tile[RoomSize, RoomSize];
            for (int x = 0; x < RoomSize; x++)
                for (int z = 0; z < RoomSize; z++)
                {
                    float worldX;
                    float worldZ;

                    worldX = ((x + transform.position.x)) - RoomSize / 2;
                    worldZ = ((z + transform.position.z)) - RoomSize / 2;

                    _tiles[x, z] = new Tile(new Vector3(worldX, 0, worldZ));
                }
        }

        private void OnDrawGizmos()
        {
            if (_tiles != default)
            {
                for (int x = 0; x < RoomSize; x++)
                    for (int z = 0; z < RoomSize; z++)
                        _tiles[x, z].DrawGizmos();
            }
        }
    }
}