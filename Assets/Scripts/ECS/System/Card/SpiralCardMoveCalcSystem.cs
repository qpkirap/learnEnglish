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
            
            Entities.WithAll<CardTag, InstanceTag, SpiralMoveTag>()
                .WithNone<CardSpiralMoveParameters>()
                .ForEach((Entity e,
                    int entityInQueryIndex,
                    CardCurrentMoveData moveData) =>
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

                    var timer = new Timer()
                    {
                        IsScalable = false,
                        IsPaused = false,
                        TotalTime = 0.4f,
                        TimeLeft = 0.4f,
                        Timescale = 1f
                    };
                
                    ecb.AddComponent(entityInQueryIndex, e, timer);
                    ecb.AddComponent(entityInQueryIndex, e, c1);
                    ecb.AddComponent(entityInQueryIndex, e, c2);
                
                }).ScheduleParallel();

            Entities.WithAll<CardTag, InstanceTag, SpiralMoveTag>().ForEach((
                Entity e,
                int entityInQueryIndex,
                ref CardCurrentMoveData moveData, 
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