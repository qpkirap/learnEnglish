using CraftCar.ECS.Components;
using CraftCar.ECS.Components.Tags;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace CraftCar.ECS.System
{
    public partial class SpiralCardMoveCalcSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            Debug.Log("NextCardSystem create");
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var time = Time.DeltaTime;

            Entities.WithAll<CardTag, InstanceTag, ClickNextButtonTag>().ForEach((
                Entity e,
                int entityInQueryIndex,
                ref CardMoveData moveData, 
                ref CardSpiralMoveParameters spiralMoveParameters) =>
            {
                var nextPosition = GetNextPosition(moveData.currentPosition, spiralMoveParameters.accumulatedTime, spiralMoveParameters.kSpiral);
                var nextScale = GetNextScale(moveData.currentLocalScale, spiralMoveParameters.accumulatedTime);
                
                var rMax = 100;
                if (math.abs( spiralMoveParameters.kSpiral - rMax) > 0.01f) spiralMoveParameters.kSpiral +=spiralMoveParameters.accumulatedTime;
                spiralMoveParameters.accumulatedTime += time * 0.5f;
                
                ecb.SetComponent(entityInQueryIndex, e, spiralMoveParameters);

                var newSpiralMove = new CardMoveProcess()
                {
                    nextPosition = nextPosition,
                    nextScale = nextScale
                };
                
                ecb.SetComponent(entityInQueryIndex, e, newSpiralMove);

            }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }

        private static float2 GetNextPosition(float2 currentPosition, float accumulatedTime, float k)
        {
            float radius = 50f;
            float rotationSpeed = 0.2f;
            
            float angle = accumulatedTime * rotationSpeed;

            float x = -k * radius * Mathf.Cos(math.degrees(angle));
            float y = -k * radius * Mathf.Sin(math.degrees(angle));

            return new float2(x, y) + currentPosition;
        }

        private static float2 GetNextScale(Vector2 currentScale, float time)
        {
            var newScale = currentScale - Vector2.one * time;
            
            return newScale;
        }
    }
}