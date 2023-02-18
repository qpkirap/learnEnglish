using Game.ECS.Components;
using Game.ECS.Components;
using Game.ECS.System.Base;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.ECS.System
{
    public partial class LinearCardMoveCalcSystem : MovementSystem
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        
        private const float moveSpeed = 2f;


        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var deltaTime = Time.DeltaTime;
           
            //start move
            Entities.WithAll<LinearMoveTag, CardCurrentMoveData>().WithNone<CardMoveProcess>().ForEach(
                (Entity e, int entityInQueryIndex, CardCurrentMoveData current, TargetMoveData target) =>
                {
                    var distance = math.distance(target.TargetMove, current.CurrentPosition);

                    var linearData = new CardMoveProcess();
                    linearData.NextPosition = current.CurrentPosition;
                    linearData.NextScale = current.CurrentLocalScale;

                    ecb.AddComponent(entityInQueryIndex, e, linearData);
                    ecb.AddComponent(entityInQueryIndex, e,
                        new LinearMoveData() { AccumulatedTime = deltaTime, InitDistanceToTarget = distance });
                }).ScheduleParallel();

            //move
            Entities.WithAll<LinearMoveTag, CardMoveProcess, CardCurrentMoveData>().ForEach(
                (Entity e, int entityInQueryIndex, CardCurrentMoveData current, TargetMoveData target,
                    ref CardMoveProcess moveData, ref LinearMoveData linearData) =>
                {
                    var item = GetNextPosition(current.CurrentPosition, target.TargetMove,
                        linearData.InitDistanceToTarget, linearData.AccumulatedTime);

                    if (linearData.IsPreLastMove)
                    {
                        ecb.RemoveComponent<CardMoveProcess>(entityInQueryIndex, e);
                        ecb.RemoveComponent<LinearMoveData>(entityInQueryIndex, e);
                        ecb.RemoveComponent<LinearMoveTag>(entityInQueryIndex, e);
                    }
                    else if (item.Item3 <= 1)
                    {
                        moveData.NextPosition = target.TargetMove;
                        moveData.NextScale = Vector2.one;

                        linearData.IsPreLastMove = true;
                        linearData.AccumulatedTime += deltaTime;
                    }
                    else
                    {
                        moveData.NextPosition = item.Item1;
                        moveData.NextScale = item.Item2;

                        linearData.AccumulatedTime += deltaTime;
                    }

                    //calc next position and scale
                    (float2, float2, float) GetNextPosition(
                        float2 currentPosition,
                        float2 target1,
                        float initDistance,
                        float accumulatedTime)
                    {
                        var time = accumulatedTime / moveSpeed;
                        time = math.clamp(time, 0, 1);

                        var calcPosition = math.lerp(currentPosition, target1, time);
                        
                        var distanceRemainder = math.distance(target1, currentPosition);

                        var percentMove = distanceRemainder / initDistance * 100;

                        return (calcPosition, GetNextScale(percentMove),
                            percentMove);

                        float2 GetNextScale(float percentMove)
                        {
                            var convert = (100 - math.clamp(percentMove, 0, 100)) / 100;

                            var x = math.clamp(convert, 0, 1);

                            return new Vector2(x, x);
                        }
                    }
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}