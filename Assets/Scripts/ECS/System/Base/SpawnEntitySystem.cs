using Game.ECS.Components;
using Game.ECS.Tags.Spawn;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Game.ECS.System.Base
{
    public partial class SpawnEntitySystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            Debug.Log("SpawnEntitySystem create");
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            Entities.WithAll<SpawnTag>().ForEach((Entity e, int entityInQueryIndex, ref SpawnData spawnData) =>
            {
                var newEntity = ecb.Instantiate(entityInQueryIndex, spawnData.prefabEntity);
                Translation translation = new Translation
                {
                    Value = spawnData.spawnPosition
                };
                ecb.SetComponent(entityInQueryIndex, newEntity, translation);
                ecb.RemoveComponent<SpawnTag>(entityInQueryIndex, e);
                
            }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}