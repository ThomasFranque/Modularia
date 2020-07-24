using LevelGeneration.Individuals;
using Pathfinding;
using EntityFactory;
using Entities.Modularius.Parts;
using UnityEngine;
using ModulariaBehaviourTree;

namespace Spawner
{
    public class EnemySpawner
    {
        public static void SpawnInRoom(Room r, SpawnerCollider sc)
        {
            int toSpawn = Random.Range(1, 6);
            for (int i = 0; i < toSpawn; i++)
            {
                Tile t;
                Vector3 position;
                Core c;
                
                t = r.RoomGrid.GetRandomTile(false);
                position = t.Position;
                c = Factory.GenerateNew();
                position.y = c.transform.position.y;

                c.transform.position = position;
                c.AttachedEntity.CurrentRoom = r;
                c.AttachedEntity.OnDeath += sc.EnemyDead;
                c.GetComponent<BehaviourTree>().RunTree();
            }
            sc.SetSpawned(toSpawn);
        }
    }
}