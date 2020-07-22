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
        // ^^^^^^

        public static readonly Vector3Int Forward = new Vector3Int(0, 0, 1);
        public static readonly Vector3Int Backwards = new Vector3Int(0, 0, -1);
        public static readonly Vector3Int Right = new Vector3Int(1, 0, 0);
        public static readonly Vector3Int Left = new Vector3Int(-1, 0, 0);
        private static readonly Vector3Int[] AllPossibleDirections =
            new Vector3Int[4] { Forward, Backwards, Left, Right };

        private List<Vector3Int> _takenLocations;
        private Dictionary<Vector3Int, Room> _allRooms;
        private Branch _currentMainBranch;

        private Vector3Int GetRandomDirection() =>
            AllPossibleDirections[Random.Range(0, AllPossibleDirections.Length)];

        private bool DirectionChangeRoll =>
            Random.Range(0f, 1.0f) < DIRECTION_CHANGE_CHANCE;
        private bool NewBranchRoll =>
            Random.Range(0f, 1.0f) < NEW_BRANCH_CHANCE;

        private void Awake()
        {
            Grid.SetLayermask(_pathfindingObstacles);
            CreateNewLevel();
        }

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

                // Try to create new branch
                if (canHaveBranch && HasOpenNeighbor(newLoc) && NewBranchRoll)
                {
                    Branch subBranch = new Branch(containingBranch);
                    containingBranch.AddNewSubBranch(subBranch);
                    Generate(subBranch, newLoc, newDir, maxLength / 4, 0, false);
                }
            }
        }

        // Based on the Fisher–Yates shuffle
        // taken from https://stackoverflow.com/questions/273313/randomize-a-listt
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

        private bool HasOpenNeighbor(Vector3Int toCheck)
        {
            return !PositionOccupied(toCheck - Forward) ||
                !PositionOccupied(toCheck - Backwards) ||
                !PositionOccupied(toCheck - Right) ||
                !PositionOccupied(toCheck - Left);
        }

        private bool PositionOccupied(Vector3Int position) =>
            _takenLocations.Contains(position);

        private void OnDrawGizmos()
        {
            if (_currentMainBranch != default)
                _currentMainBranch.RenderGizmos();
        }
    }
}