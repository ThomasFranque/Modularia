using System;
using System.Collections.Generic;
using LevelGeneration.Individuals;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;
using Grid = Pathfinding.Grid;

namespace LevelGeneration
{
    public class LevelGenerator : MonoBehaviour
    {
        // To be moved to a scriptable obj vvvvvv
        private const float DIRECTION_CHANGE_CHANCE = 0.5f;
        private const float NEW_BRANCH_CHANCE = 0.3f;
        [SerializeField] private LayerMask _pathfindingObstacles = default;
        [SerializeField] private GameObject _levelFinishPrefab = default;
        // ^^^^^^

        public static readonly Vector3Int Forward = new Vector3Int(0, 0, 1);
        public static readonly Vector3Int Backwards = new Vector3Int(0, 0, -1);
        public static readonly Vector3Int Right = new Vector3Int(1, 0, 0);
        public static readonly Vector3Int Left = new Vector3Int(-1, 0, 0);
        private static readonly Vector3Int[] AllPossibleDirections =
            new Vector3Int[4] { Forward, Backwards, Left, Right };

        [SerializeField] private int _seed = default;

        private List<Vector3Int> _takenLocations;
        private Dictionary<Vector3Int, Room> _allRooms;
        private Branch _currentMainBranch;

        private bool _exitPlaced;

        private Vector3Int GetRandomDirection() =>
            AllPossibleDirections[Random.Range(0, AllPossibleDirections.Length)];

        private bool DirectionChangeRoll =>
            Random.Range(0f, 1.0f) < DIRECTION_CHANGE_CHANCE;
        private bool NewBranchRoll =>
            Random.Range(0f, 1.0f) < NEW_BRANCH_CHANCE;

        private void Awake()
        {
            Grid.SetLayermask(_pathfindingObstacles);
            CreateNewLevel(_seed);
        }

        /// <summary>
        /// Creates a new level
        /// </summary>
        /// <param name="seed">Level Seed</param>
        public void CreateNewLevel(int seed = default)
        {
            /*
            How this works:
            The Generate() method will generate the main path recursively.
            When the main path is complete, for every created room, it will try 
            to create a new branch.
            When all the positions are in place, tell the branches to spawn the
            rooms.
            */

            // Set seed
            if (seed == default)
                seed = Random.Range(int.MinValue, int.MaxValue);
            else
                Random.InitState(seed);

            // Initialize collections
            _takenLocations = new List<Vector3Int>();
            _allRooms = new Dictionary<Vector3Int, Room>();

            // Generate
            Branch mainBranch = new Branch();
            _takenLocations.Add(Vector3Int.zero);
            mainBranch.AddNewRoomPosition(Vector3Int.zero);
            _exitPlaced = false;
            Generate(mainBranch, Vector3Int.zero, Forward, 30, 0, true);

            // Set parents
            Transform parentTransform =
                mainBranch.SpawnRooms(_allRooms);
            parentTransform.name = "Main Branch";

            // Open doors
            for (int i = 0; i < _takenLocations.Count; i++)
            {
                _allRooms[_takenLocations[i]]
                    .OpenConnections();
            }

            // Finalize
            _currentMainBranch = mainBranch;
            GC.Collect();
        }

        /// <summary>
        /// Recursive generator method 
        /// </summary>
        /// <param name="containingBranch">Branch in which it is contained</param>
        /// <param name="previousLoc">Previous generated location</param>
        /// <param name="previousDir">Previous generated direction</param>
        /// <param name="maxLength">Max branch length</param>
        /// <param name="count">Amount created</param>
        /// <param name="canHaveBranch">Can have sub branches</param>
        private void Generate(Branch containingBranch, Vector3Int previousLoc, Vector3Int previousDir, int maxLength, int count, bool canHaveBranch)
        {
            Vector3Int newLoc = previousLoc + previousDir;
            Vector3Int newDir = previousDir;
            // Check if it can continue the same direction
            bool selected = !PositionOccupied(newLoc);
            // Determine if it should change direction
            bool changeDir = DirectionChangeRoll || !selected;

            if (changeDir)
            {
                selected = false;
                ShuffleDirections();
                for (int i = 0; i < AllPossibleDirections.Length; i++)
                {
                    Vector3Int tempLoc = AllPossibleDirections[i] + previousLoc;
                    if (!PositionOccupied(tempLoc))
                    {
                        newDir = AllPossibleDirections[i];
                        newLoc = tempLoc;
                        selected = true;
                        break;
                    }
                }
            }

            // If the selection was possible
            if (selected && count < maxLength)
            {
                _takenLocations.Add(newLoc);
                containingBranch.AddNewRoomPosition(newLoc);
                Generate(containingBranch, newLoc, newDir, maxLength, count + 1, canHaveBranch);

                // Try yo place the exit
                if (!_exitPlaced)
                {
                    _exitPlaced = true;
                    PlaceExit(newLoc);
                }

                // Try to create new branch
                if (canHaveBranch && HasOpenNeighbor(newLoc) && NewBranchRoll)
                {
                    Branch subBranch = new Branch(containingBranch);
                    containingBranch.AddNewSubBranch(subBranch);
                    Generate(subBranch, newLoc, newDir, maxLength / 4, 0, false);
                }
            }
        }

        /// <summary>
        /// Place the exit
        /// </summary>
        /// <param name="position"></param>
        private void PlaceExit(Vector3Int position)
        {
            GameObject exit =
                Instantiate(_levelFinishPrefab);
            exit.transform.position = position * Room.ROOM_SIZE;
        }

        /// <summary>
        /// Shuffle directions collection
        /// </summary>
        private void ShuffleDirections()
        {
            int n = AllPossibleDirections.Length;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                Vector3Int value = AllPossibleDirections[k];
                AllPossibleDirections[k] = AllPossibleDirections[n];
                AllPossibleDirections[n] = value;
            }
        }

        /// <summary>
        /// Checks if a position has an open neighbor
        /// </summary>
        /// <param name="toCheck">Position to check</param>
        /// <returns>True if has a open neighbor</returns>
        private bool HasOpenNeighbor(Vector3Int toCheck)
        {
            return !PositionOccupied(toCheck - Forward) ||
                !PositionOccupied(toCheck - Backwards) ||
                !PositionOccupied(toCheck - Right) ||
                !PositionOccupied(toCheck - Left);
        }

        /// <summary>
        /// Check if the position is occupied
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool PositionOccupied(Vector3Int position) =>
            _takenLocations.Contains(position);

        private void OnDrawGizmos()
        {
            if (_currentMainBranch != default)
                _currentMainBranch.RenderGizmos();
        }
    }
}