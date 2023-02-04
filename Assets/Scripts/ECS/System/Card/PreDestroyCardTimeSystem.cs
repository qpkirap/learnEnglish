using CraftCar.ECS.Components;
using CraftCar.ECS.Components.Tags;
using Game.ECS.System.Base;
using Unity.Entities;

namespace Game.ECS.System
{
    public partial class PreDestroyCardTimeSystem : PreDestroySystem
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities.WithAll<Timer>().WithNone<DestroyTag>()
                .ForEach((Entity e, int entityInQueryIndex, Timer timer) =>
            {
                if (timer.IsCompleted) ecb.AddComponent(entityInQueryIndex, e, new DestroyTag());
            }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}