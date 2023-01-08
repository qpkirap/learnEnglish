using CraftCar.ECS.Components;
using CraftCar.ECS.Components.Tags;
using Unity.Entities;

namespace CraftCar.ECS.System.Card
{
    public partial class CardCreateSpiralMoveSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        
        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var time = Time.DeltaTime;
            
            Entities.WithAll<CardTag, InstanceTag, ClickNextButtonTag>()
                .WithNone<CardSpiralMoveParameters>()
                .ForEach((Entity e,
                    int entityInQueryIndex,
                    CardMoveData moveData) =>
                {
                    var c1 = new CardSpiralMoveParameters()
                    {
                        accumulatedTime = 0,
                        kSpiral = 0
                    };
                    var c2 = new CardMoveProcess()
                    {
                        nextPosition = moveData.currentPosition,
                        nextScale = moveData.currentLocalScale
                    };
                
                    ecb.AddComponent(entityInQueryIndex, e, c1);
                    ecb.AddComponent(entityInQueryIndex, e, c2);
                
                }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}