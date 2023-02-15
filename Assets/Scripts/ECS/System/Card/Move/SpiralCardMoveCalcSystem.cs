using CraftCar.ECS.Components;
using Game.ECS.System.Base;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.ECS.System
{
    public partial class SpiralCardMoveCalcSystem : MovementSystem
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        private const float speedScaleRadius = 100;

        protected override void OnCreate()
        {
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
                    CardCurrentMoveData moveData,
                    RandomData randomData) =>
                {
                    var roll = randomData.random ? 1 : -1;

                    var c1 = new CardSpiralMoveParameters()
                    {
                        accumulatedTime = 0,
                        kSpiral = 0,
                        random = roll
                    };
                    var c2 = new CardMoveProcess()
                    {
                        nextPosition = moveData.currentPosition,
                        nextScale = moveData.currentLocalScale
                    };
                    
                    ecb.AddComponent(entityInQueryIndex, e, c1);
                    ecb.AddComponent(entityInQueryIndex, e, c2);
                }).ScheduleParallel();

            Entities.WithAll<CardSpiralMoveParameters, InstanceTag, SpiralMoveTag>().ForEach((
                Entity e,
                int entityInQueryIndex,
                CardCurrentMoveData moveData,
                ref CardSpiralMoveParameters spiralMoveParameters,
                ref CardMoveProcess cardMoveProcess) =>
            {
                var nextPosition = GetNextPosition(moveData.currentPosition, spiralMoveParameters.accumulatedTime,
                    spiralMoveParameters.kSpiral * spiralMoveParameters.random);
                var nextScale = GetNextScale(moveData.currentLocalScale, time);

                spiralMoveParameters.kSpiral += time * speedScaleRadius;

                spiralMoveParameters.accumulatedTime += time;

                cardMoveProcess.nextPosition = nextPosition;
                cardMoveProcess.nextScale = nextScale;
            }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }

        private static float2 GetNextPosition(float2 currentPosition, float accumulatedTime, float kSpiral)
        {
            float rotationSpeed = 0.2f;

            float angle = accumulatedTime * rotationSpeed;

            float x = kSpiral * Mathf.Cos(math.degrees(angle));
            float y = kSpiral * Mathf.Sin(math.degrees(angle));

            return new float2(x, y) + currentPosition;
        }

        private static float2 GetNextScale(Vector2 currentScale, float time)
        {
            var newScale = currentScale - Vector2.one * time;

            return newScale;
        }
    }
}