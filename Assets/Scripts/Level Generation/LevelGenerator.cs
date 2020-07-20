using System.Collections.Generic;
using LevelGeneration.Individuals;
using UnityEngine;

namespace LevelGeneration
{
    public class LevelGenerator : MonoBehaviour
    {
        // To be moved to a scriptable obj vvvvvv
        private const float DIRECTION_CHANGE_CHANCE = 0.4f;
        private const float NEW_BRANCH_CHANCE = 0.2f;
        private const int MAX_BRANCH_LENGTH = 20;
        // ^^^^^^

        private static readonly Vector3Int Forward = new Vector3Int(0, 0, 1);
        private static readonly Vector3Int Backwards = new Vector3Int(0, 0, -1);
        private static readonly Vector3Int Right = new Vector3Int(1, 0, 0);
        private static readonly Vector3Int Left = new Vector3Int(-1, 0, 0);
        private static readonly Vector3Int[] AllPossibleDirections =
            new Vector3Int[4] { Forward, Backwards, Left, Right };

        private List<Vector3Int> _takenLocation;

        private Vector3Int GetRandomDirection() =>
            AllPossibleDirections[Random.Range(0, AllPossibleDirections.Length)];

        private bool DirectionChangeRoll =>
            Random.Range(0f, 1.0f) < DIRECTION_CHANGE_CHANCE;
        private bool NewBranchRoll =>
            Random.Range(0f, 1.0f) < NEW_BRANCH_CHANCE;

        private void Awake()
        {
            CreateNewLevel();
        }

        public void CreateNewLevel()
        {
            _takenLocation = new List<Vector3Int>();
            Branch mainBranch = new Branch();

            _takenLocation.Add(Vector3Int.zero);

            Generate(Vector3Int.zero, Forward);
        }

        private void Generate(Vector3Int previousLoc, Vector3Int previousDir, int count = 0)
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

            // Try to create new branch
            if (HasOpenNeighbor(newLoc) && NewBranchRoll)
                Generate(newLoc, newDir, 0);

            // If the selection was possible
            if (selected && count < MAX_BRANCH_LENGTH)
            {
                _takenLocation.Add(newLoc);
                Generate(newLoc, newDir, count + 1);
            }
        }

        private void GenerateNewRoom(Vector3Int from)
        {

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
            _takenLocation.Contains(position);

        private void OnDrawGizmos()
        {
            if (_takenLocation != default)
            {
                Vector3Int previous = Vector3Int.zero;
                foreach (Vector3Int v in _takenLocation)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(v * 30, new Vector3(30, 30, 30));
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(previous * 30, v * 30);
                    previous = v;
                }
            }
        }
    }
}