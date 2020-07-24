using UnityEngine;
using Grid = Pathfinding.Grid;
using Pathfinding;
using TMPro;

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

        [Space]

        [SerializeField] private TextMeshPro _roomInfo = default;
        [SerializeField] private GameObject[] _propsPrefabs = default;
        [SerializeField, ReadOnly] private bool _isBranchStart = default;
        [SerializeField, ReadOnly] private bool _isBranchEnd = default;

        private Vector3Int _position;
        private(bool f, bool b, bool l, bool r) _doorStates;
        public Branch ParentBranch { get; private set; }
        public Grid RoomGrid { get; private set; }
        public bool Initialized { get; private set; }

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
            RoomGrid.Generate(this);
            SpawnProps();
            _roomInfo.text = position.ToString();;
            Initialized = true;
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

            _doorStates = (frontOpen, backOpen, leftOpen, rightOpen);
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

        private void SpawnProps()
        {
            int toSpawn = Random.Range(0, 4);

            for (int i = 0; i < toSpawn; i++)
            {
                Vector3 position;
                Tile t;
                t = RoomGrid.GetRandomTile(false);
                position = t.Position;
                GameObject spawned =
                    Instantiate(
                        _propsPrefabs[Random.Range(0, _propsPrefabs.Length)],
                        transform);
                position.y = spawned.transform.position.y;
                spawned.transform.position = position;
            }
        }

        public void CloseDoors()
        {
            OpenDoors(false, false, false, false);
        }

        public void OpenDoors()
        {
            OpenDoors(_doorStates.f, _doorStates.b, _doorStates.l, _doorStates.r);
        }
    }
}