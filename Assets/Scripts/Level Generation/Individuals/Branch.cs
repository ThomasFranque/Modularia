using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration.Individuals
{
    public class Branch
    {
        private static GameObject RoomPrefab;
        private Branch _parentBranch;
        public bool IsMain { get; }
        public List<Branch> SubBranches { get; private set; }
        public List<Vector3Int> RoomPositions { get; private set; }
        public Dictionary<Vector3Int, Room> BranchRooms { get; private set; }
        public Dictionary<Vector3Int, Room> AllRooms { get; private set; }
        private Transform _parentTransform;

        private int RoomSize => Room.ROOM_SIZE;

        public Branch()
        {
            IsMain = true;
            _parentBranch = null;
            InitializeCollections();
        }

        public Branch(Branch parentBranch)
        {
            IsMain = false;
            _parentBranch = parentBranch;
            InitializeCollections();
        }

        private void InitializeCollections()
        {
            if (RoomPrefab == null)
                RoomPrefab = Resources.Load<GameObject>(Room.PREFAB_PATH);

            AllRooms = default;
            SubBranches = new List<Branch>();
            RoomPositions = new List<Vector3Int>();
            BranchRooms = new Dictionary<Vector3Int, Room>();
        }

        public void AddNewRoomPosition(Vector3Int roomPosition)
        {
            RoomPositions.Add(roomPosition);
        }

        public void AddNewSubBranch(Branch newBranch)
        {
            SubBranches.Add(newBranch);
        }

        public Transform SpawnRooms(Dictionary<Vector3Int, Room> allRooms,
            Transform parent = null)
        {
            // Update all rooms
            AllRooms = allRooms;
            // Create the parent
            _parentTransform = new GameObject("Branch").transform;
            // Set the parent of the created parent
            _parentTransform.SetParent(parent);

            int collectionSize = RoomPositions.Count - 1;
            // Spawn the rooms
            for (int i = collectionSize; i >= 0; i--)
            {
                // Instantiate the room and get the Room component
                Vector3Int pos = RoomPositions[i];
                Room spawnedRoom =
                    GameObject.Instantiate(RoomPrefab, _parentTransform)
                    .GetComponent<Room>();

                // Initialize it
                spawnedRoom.Initialize(pos, this, i == collectionSize, i == 0);

                // Add them to the dictionary
                BranchRooms.Add(pos, spawnedRoom);
                allRooms.Add(pos, spawnedRoom);
            }

            // Tell sub-branches to spawn them
            for (int i = SubBranches.Count - 1; i >= 0; i--)
            {
                Branch b = SubBranches[i];
                b.SpawnRooms(allRooms, _parentTransform);
            }

            return _parentTransform;
        }

        public void RenderGizmos()
        {
            Vector3Int previousPos = RoomPositions[0];

            for (int i = RoomPositions.Count - 1; i >= 0; i--)
            {
                Vector3Int v = RoomPositions[i];
                // Gizmos.color = IsMain ? Color.cyan : Color.white;
                // Gizmos.DrawWireCube(v * RoomSize, new Vector3(RoomSize, RoomSize, RoomSize));
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(previousPos * RoomSize, v * RoomSize);
                // Gizmos.DrawSphere(RoomPositions[0] * RoomSize, 2.5f);
                previousPos = v;
            }

            for (int i = SubBranches.Count - 1; i >= 0; i--)
            {
                Branch b = SubBranches[i];
                b.RenderGizmos();
            }
        }
    }
}