using UnityEngine;
using Grid = Pathfinding.Grid;

namespace LevelGeneration.Individuals
{
    public class Room : MonoBehaviour
    {
        public const int ROOM_SIZE = 30;
        public const string PREFAB_PATH =
            "Prefabs/Generation/Room";

        [Header("Doors")]
        [SerializeField] private GameObject _frontDoor = default;
        [SerializeField] private GameObject _backDoor = default;
        [SerializeField] private GameObject _rightDoor = default;
        [SerializeField] private GameObject _leftDoor = default;
        [Header("Doors")]
        [SerializeField] private GameObject _frontDestructable = default;
        [SerializeField] private GameObject _backDestructable = default;
        [SerializeField] private GameObject _rightDestructable = default;
        [SerializeField] private GameObject _leftDestructable = default;
        [SerializeField, ReadOnly] private bool _isBranchStart = default;
        [SerializeField, ReadOnly] private bool _isBranchEnd = default;

        private Vector3Int _position;
        public Branch ParentBranch { get; private set; }
        public Grid RoomGrid { get; private set; }

        public bool IsBranchStart => _isBranchStart;
        public bool IsBranchEnd => _isBranchEnd;
        public Vector3Int Position => _position;

        public void Initialize(Vector3Int position, Branch parentBranch, bool isBranchStart, bool isBranchEnd)
        {
            _position = position;
            _isBranchStart = isBranchStart;
            _isBranchEnd = IsBranchEnd;
            transform.position = position * ROOM_SIZE;
            ParentBranch = parentBranch;
            RoomGrid = gameObject.AddComponent<Grid>();
            RoomGrid.Generate();
        }

        public void OpenConnections()
        {
            Room frontRoom;
            Room backRoom;
            Room leftRoom;
            Room rightRoom;

            bool frontOpen = false;
            bool backOpen = false;
            bool leftOpen = false;
            bool rightOpen = false;

            if (ParentBranch.AllRooms
                .TryGetValue(_position + LevelGenerator.Forward, out frontRoom))
            {
                frontOpen =
                    frontRoom.IsOfBranch(ParentBranch) ||
                    frontRoom.IsBranchStart ||
                    IsBranchStart;
            }
            if (ParentBranch.AllRooms
                .TryGetValue(_position + LevelGenerator.Backwards, out backRoom))
            {
                backOpen =
                    backRoom.IsOfBranch(ParentBranch) ||
                    backRoom.IsBranchStart ||
                    IsBranchStart;
            }
            if (ParentBranch.AllRooms
                .TryGetValue(_position + LevelGenerator.Left, out leftRoom))
            {
                leftOpen =
                    leftRoom.IsOfBranch(ParentBranch) ||
                    leftRoom.IsBranchStart ||
                    IsBranchStart;
            }
            if (ParentBranch.AllRooms
                .TryGetValue(_position + LevelGenerator.Right, out rightRoom))
            {
                rightOpen =
                    rightRoom.IsOfBranch(ParentBranch) ||
                    rightRoom.IsBranchStart ||
                    IsBranchStart;
            }

            OpenDoors(frontOpen, backOpen, leftOpen, rightOpen);
        }

        public bool IsOfBranch(Branch toCheck) =>
            toCheck == ParentBranch;

        private void OpenDoors(bool front, bool back, bool left, bool right)
        {
            _frontDoor.SetActive(!front);
            _backDoor.SetActive(!back);
            _leftDoor.SetActive(!left);
            _rightDoor.SetActive(!right);
        }
    }
}