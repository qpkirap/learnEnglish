using CraftCar.ECS.Components;
using CraftCar.ECS.Components.Tags;
using Unity.Entities;

namespace CraftCar.ECS.System.Card
{
    public partial class PreDestroyCardTimeSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities.WithAll<Timer, CardTag, InstanceTag>().WithNone<DestroyTag>()
                .ForEach((Entity e, int entityInQueryIndex, Timer timer) =>
            {
                if(timer.IsCompleted) ecb.AddComponent(entityInQueryIndex, e, new DestroyTag());
            }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}