using CraftCar.ECS.Components;
using Game.ECS.System.Base;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.ECS.System
{
    public partial class SpiralCardMoveCalcSystem : MovementSystem
    {
        private EndSimulationEntityCommandBufferSystem EntityCommandBufferSystem;

        private const float SpeedScaleRadius = 100;
        private const float MaxSpiralRadius = 36;
        private const float RotationSpeed = .1f;
        private const float ScaleSpeed = 1.1f;

        protected override void OnCreate()
        {
            EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var time = Time.DeltaTime;

            Entities.WithAll<CardTag, InstanceTag, SpiralMoveTag>()
                .WithNone<CardSpiralMoveParameters>()
                .ForEach((Entity e,
                    int entityInQueryIndex,
                    CardCurrentMoveData moveData,
                    RandomData randomData) =>
                {
                    var roll = randomData.random ? 1 : -1;

                    var cSpiralMove = new CardSpiralMoveParameters()
                    {
                        AccumulatedTime = 0,
                        KSpiral = 0,
                        Random = roll
                    };
                    var cMoveProc = new CardMoveProcess()
                    {
                        nextPosition = moveData.currentPosition,
                        nextScale = moveData.currentLocalScale,
                        nextLocalRotation = quaternion.LookRotation(Vector3.zero, Vector3.up)
                    };

                    ecb.AddComponent(entityInQueryIndex, e, cSpiralMove);
                    ecb.AddComponent(entityInQueryIndex, e, cMoveProc);
                    
                }).ScheduleParallel();

            Entities.WithAll<CardSpiralMoveParameters, InstanceTag, SpiralMoveTag>().ForEach((
                Entity e,
                int entityInQueryIndex,
                CardCurrentMoveData moveData,
                ref CardSpiralMoveParameters spiralMoveParameters,
                ref CardMoveProcess cardMoveProcess) =>
            {
                var calcSpiral = math.clamp(spiralMoveParameters.KSpiral * spiralMoveParameters.Random,
                    -MaxSpiralRadius, MaxSpiralRadius);

                var nextPosition = GetNextPosition(moveData.currentPosition, spiralMoveParameters.AccumulatedTime,
                    calcSpiral);
                var nextScale = GetNextScale(moveData.currentLocalScale, time);

                spiralMoveParameters.KSpiral += time * SpeedScaleRadius;
                spiralMoveParameters.AccumulatedTime += time;
                
                cardMoveProcess.nextPosition = nextPosition;
                cardMoveProcess.nextScale = nextScale;
                
            }).ScheduleParallel();

            EntityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }

        private static float2 GetNextPosition(float2 currentPosition, float accumulatedTime, float kSpiral)
        {
            float angle = accumulatedTime * RotationSpeed;

            float x = kSpiral * Mathf.Cos(math.degrees(angle));
            float y = kSpiral * Mathf.Sin(math.degrees(angle));

            return new float2(x, y) + currentPosition;
        }

        private static float2 GetNextScale(Vector2 currentScale, float time)
        {
            var newScale = math.clamp(currentScale - Vector2.one * time * ScaleSpeed,
                Vector2.zero,
                Vector2.one);

            return newScale;
        }
    }
}