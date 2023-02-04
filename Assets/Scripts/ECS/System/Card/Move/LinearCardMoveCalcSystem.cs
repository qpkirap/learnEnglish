using CraftCar.ECS.Components;
using CraftCar.ECS.Components.Tags;
using Game.ECS.System.Base;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.ECS.System
{
    public partial class LinearCardMoveCalcSystem : MovementSystem
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        private const float minCardSpeed = 20f;
        private const float maxCardSpeed = 100f;
        private const float scaleSpeed = 0.3f;
        

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var time = Time.DeltaTime;

            Entities.WithAll<LinearMoveTag, CardCurrentMoveData>().WithNone<CardMoveProcess>().ForEach(
                (Entity e, int entityInQueryIndex, CardCurrentMoveData current, TargetMoveData target) =>
                {
                    var distance = math.distance(target.TargetMove, current.currentPosition);

                    var linearData = new CardMoveProcess();
                    var item = GetNextPosition(current.currentPosition, current.currentLocalScale, target.TargetMove,
                        distance);
                    linearData.nextPosition = item.Item1;
                    linearData.nextScale = item.Item2;

                    ecb.AddComponent(entityInQueryIndex, e, linearData);
                    ecb.AddComponent(entityInQueryIndex, e,
                        new LinearMoveData() { accumulatedTime = time, initDistanceToTarget = distance });
                }).ScheduleParallel();

            Entities.WithAll<LinearMoveTag, CardMoveProcess, CardCurrentMoveData>().ForEach(
                (Entity e, int entityInQueryIndex, CardCurrentMoveData current, TargetMoveData target,
                    ref CardMoveProcess moveData, ref LinearMoveData linearData) =>
                {
                    var distance = math.distance(target.TargetMove, current.currentPosition);

                    if (distance <= 0.7f)
                    {
                        ecb.RemoveComponent<CardMoveProcess>(entityInQueryIndex, e);
                        ecb.RemoveComponent<LinearMoveData>(entityInQueryIndex, e);
                        ecb.RemoveComponent<LinearMoveTag>(entityInQueryIndex, e);
                    }
                    else
                    {
                        var item = GetNextPosition(current.currentPosition, current.currentLocalScale,
                            target.TargetMove, linearData.initDistanceToTarget);

                        var checkDistance = math.distance(item.Item1, target.TargetMove);

                        moveData.nextPosition = checkDistance <= 0.7f ? target.TargetMove : item.Item1;
                        moveData.nextScale = checkDistance <= 0.7f ? Vector2.one : item.Item2;

                        linearData.accumulatedTime += time;
                    }
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }

        /// <summary>
        /// Position, Scale
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="currentScale"></param>
        /// <param name="target"></param>
        /// <param name="initDistance"></param>
        /// <returns></returns>
        private static (float2, float2) GetNextPosition(float2 currentPosition, float2 currentScale, float2 target,
            float initDistance)
        {
            var distance = math.distance(target, currentPosition);

            var checkSpeed = math.clamp(distance / 10, minCardSpeed, maxCardSpeed);

            var normalize = math.normalize(target - currentPosition);

            if (distance <= 10)
            {
                return (currentPosition + normalize * distance / 3, GetNextScale(distance, scaleSpeed, initDistance));
            }

            if (distance <= 3)
            {
                return (currentPosition + normalize * distance, GetNextScale(distance, scaleSpeed, initDistance));
            }

            return (normalize * checkSpeed + currentPosition, GetNextScale(distance, scaleSpeed, initDistance));

            float2 GetNextScale(float currentDistance, float scaleSpeed, float startDistance)
            {
                var x = math.clamp(startDistance * math.clamp(scaleSpeed, 0.1f, 1) / currentDistance, 0, 1);

                return new Vector2(x, x);
            }
        }
    }
}