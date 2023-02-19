using Game.ECS.Components;
using Unity.Entities;

namespace Game.ECS.System
{
    public partial class NextCardSystem : UpdateSystem
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Entities
                .WithAll<ClickNextButtonTag, InstanceTag>()
                .WithNone<PreDestroyTag>()
                .ForEach((Entity e, int entityInQueryIndex) =>
                {
                    ecb.AddComponent<PreDestroyTag>(entityInQueryIndex, e);
                }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}