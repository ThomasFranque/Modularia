using LevelGeneration.Individuals;
using UnityEngine;

namespace Spawner
{
    [RequireComponent(typeof(Collider))]
    public class SpawnerCollider : MonoBehaviour
    {
        [SerializeField] private Room _room = default;
        [SerializeField] private GameObject _light = default;
        private int _spawned;
        private int _killed;

        private void Start()
        {
            if (_room.IsBranchEnd || _room.IsBranchStart)
            {
                gameObject.SetActive(false);
                _light.SetActive(true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_room.Initialized) return;
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                TriggerRoomEntry();
        }

        private void TriggerRoomEntry()
        {
            _light.SetActive(true);
            EnemySpawner.SpawnInRoom(_room, this);
            gameObject.SetActive(false);
            _room.CloseDoors();
        }

        public void SetSpawned(int amount)
        {
            _spawned = amount;
        }

        public void EnemyDead()
        {
            _killed++;
            if (_killed == _spawned)
                RoomCleared();
        }

        private void RoomCleared()
        {
            _room.OpenDoors();
        }
    }
}