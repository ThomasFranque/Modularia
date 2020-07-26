using System;
using System.Collections.Generic;
using LevelGeneration.Individuals;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pathfinding
{
    public class Grid : MonoBehaviour
    {
        public static LayerMask ObstaclesMask { get; private set; }
        private static int RoomSize => Room.ROOM_SIZE;

        private Tile[, ] _tiles;
        private Room AttachedRoom { get; set; }

        /// <summary>
        /// Sets the layermask for the tiles to use
        /// </summary>
        /// <param name="l">obstacles layermask</param>
        public static void SetLayermask(LayerMask l)
        {
            ObstaclesMask = l;
        }

        /// <summary>
        /// Generates a room grid with a predefined size
        /// </summary>
        /// <param name="attachedRoom">Target room</param>
        public void Generate(Room attachedRoom)
        {
            // Assign attached
            AttachedRoom = attachedRoom;
            // Initialize tiles collection
            _tiles = new Tile[RoomSize, RoomSize];
            // Generate tiles
            for (int x = 0; x < RoomSize; x++)
                for (int z = 0; z < RoomSize; z++)
                {
                    // Transform local coordinates to world coordinates
                    float worldX;
                    float worldZ;
                    Vector3 worldPosition;

                    worldX = x + transform.position.x;
                    worldZ = z + transform.position.z;

                    worldX -= RoomSize / 2;
                    worldZ -= RoomSize / 2;

                    worldPosition = new Vector3(worldX, 0, worldZ);

                    // Create a new tile
                    _tiles[x, z] = new Tile(worldPosition, x, z);
                }

            AssignNeighbors();

            // Call garbage collector for good measure
            GC.Collect();
        }

        /// <summary>
        /// Using the created tile collection, assign each tile's neighbors
        /// </summary>
        private void AssignNeighbors()
        {
            foreach (Tile t in _tiles)
            {
                List<Tile> neighbors = new List<Tile>(8);
                for (int x = -1; x <= 1; x++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x == 0 && z == 0) continue;

                        int neighborX;
                        int neighborZ;

                        neighborX = t.CollectionIndex.x + x;
                        neighborZ = t.CollectionIndex.z + z;

                        if (IndexInsideBounds(neighborX, neighborZ))
                            neighbors.Add(_tiles[neighborX, neighborZ]);
                    }
                }
                t.SetNeighbors(neighbors.ToArray());
            }
        }

        // Help from https://www.youtube.com/watch?v=mZfyt03LDH4
        /// <summary>
        /// Pathfinds a path to the desired position
        /// </summary>
        /// <param name="from">Starting point</param>
        /// <param name="to">End point</param>
        /// <returns>Final path</returns>
        public Vector3[] GetPath(Vector3 from, Vector3 to)
        {
            Tile startTile;
            Tile endTile;
            List<Tile> openTiles;
            HashSet<Tile> closedTiles;

            openTiles = new List<Tile>();
            closedTiles = new HashSet<Tile>();

            // Get tile from world
            GetTileFromWorldPosition(from, out startTile);
            GetTileFromWorldPosition(to, out endTile);

            // Add the initial tile
            openTiles.Add(startTile);
            int iteration = 0;
            while (openTiles.Count > 0)
            {
                // If the iteration is stuck, stop it
                if (iteration == RoomSize * RoomSize) break;
                iteration++;
                Tile current = openTiles[0];

                // Check if current is worse than any open
                for (int i = 1; i < openTiles.Count; i++)
                    if (current.IsWorseThan(openTiles[i]))
                        current = openTiles[i];

                // Update collections
                openTiles.Remove(current);
                closedTiles.Add(current);

                // If we reached our destination
                if (current == endTile)
                {
                    //Debug.Log(iteration);
                    return GetFinalPath(startTile, endTile);
                }

                // Run through neighbors
                for (int i = 0; i < current.Neighbors.Length; i++)
                {
                    Tile neighbor = current.Neighbors[i];
                    bool notYetAdded = !openTiles.Contains(neighbor);

                    // Update occupied state
                    neighbor.UpdateState();

                    // Should we take it into account
                    if ((neighbor.Obstructed && neighbor != endTile) || !notYetAdded) continue;

                    // Get new beta
                    float newBeta = current.Beta + current.DistanceTo(neighbor.Position);
                    // Compare betas
                    if (newBeta < neighbor.Beta || notYetAdded)
                    {
                        // Update new best
                        neighbor.Beta = newBeta;
                        neighbor.Alpha = neighbor.DistanceTo(endTile.Position);
                        neighbor.Parent = current;
                        if (notYetAdded)
                            openTiles.Add(neighbor);
                    }
                }
            }

            // In case of fail, return empty collection
            return new Vector3[0];
        }

        /// <summary>
        /// Gets a final path from the start tile
        /// </summary>
        /// <param name="start">Start tile</param>
        /// <param name="end">End Tile</param>
        /// <returns>Final path</returns>
        private Vector3[] GetFinalPath(Tile start, Tile end)
        {
            List<Vector3> positions = new List<Vector3>();
            Tile current = start;
            int maxIters = RoomSize * RoomSize + 1;
            int iters = 0;
            while (current != end && current != null)
            {
                positions.Add(current.Position);
                //Debug.Log(current.CollectionIndex.x + " , " + current.CollectionIndex.z);
                current = current.Parent;
                iters++;
                if (iters > maxIters) break;
            }
            //positions.Reverse();
            return positions.ToArray();
        }

        /// <summary>
        /// Transform world position to tile position
        /// </summary>
        /// <param name="worldPos">world position</param>
        /// <param name="tile">Relative tile</param>
        private void GetTileFromWorldPosition(Vector3 worldPos, out Tile tile)
        {
            int x;
            int z;
            int halfRoomSize = RoomSize / 2;

            x = Mathf.RoundToInt(worldPos.x - transform.position.x);
            x += halfRoomSize;
            z = Mathf.RoundToInt(worldPos.z - transform.position.z);
            z += halfRoomSize;

            tile = _tiles[x, z];
        }

        /// <summary>
        /// Gets a random tile in the grid
        /// </summary>
        /// <param name="obstructed">Obstructed state</param>
        /// <returns>A random tile</returns>
        public Tile GetRandomTile(bool obstructed)
        {
            int index1;
            int index2;
            do
            {
                index1 = Random.Range(0, _tiles.GetLength(0));
                index2 = Random.Range(0, _tiles.GetLength(1));
                _tiles[index1, index2].UpdateState();
            } while (_tiles[index1, index2].Obstructed != obstructed);
            return _tiles[index1, index2];
        }

        /// <summary>
        /// Check if given index is inside of the collection bounds
        /// </summary>
        /// <param name="x">X to check</param>
        /// <param name="z">Z to check</param>
        /// <returns>True if inside</returns>
        private bool IndexInsideBounds(int x, int z) =>
            x >= 0 && z >= 0 &&
            x < RoomSize && z < RoomSize;

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