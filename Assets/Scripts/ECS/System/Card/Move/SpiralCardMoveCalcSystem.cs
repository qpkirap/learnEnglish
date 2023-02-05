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
                var nextScale = GetNextScale(moveData.currentLocalScale, spiralMoveParameters.accumulatedTime);

                var rMax = 100;
                if (math.abs(spiralMoveParameters.kSpiral - rMax) > 0.01f)
                    spiralMoveParameters.kSpiral += spiralMoveParameters.accumulatedTime;

                spiralMoveParameters.accumulatedTime += time * 0.5f;

                cardMoveProcess.nextPosition = nextPosition;
                cardMoveProcess.nextScale = nextScale;
            }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }

        private static float2 GetNextPosition(float2 currentPosition, float accumulatedTime, float k)
        {
            float radius = 50;
            float rotationSpeed = 0.2f;

            float angle = accumulatedTime * rotationSpeed;

            float x = k * radius * Mathf.Cos(math.degrees(angle));
            float y = k * radius * Mathf.Sin(math.degrees(angle));

            return new float2(x, y) + currentPosition;
        }

        private static float2 GetNextScale(Vector2 currentScale, float time)
        {
            var newScale = currentScale - Vector2.one * time;

            return newScale;
        }
    }
}