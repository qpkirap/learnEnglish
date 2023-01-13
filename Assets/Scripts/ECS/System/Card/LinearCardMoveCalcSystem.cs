using CraftCar.ECS.Components;
using CraftCar.ECS.Components.Tags;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace CraftCar.ECS.System.Card
{
    public partial class LinearCardMoveCalcSystem : SystemBase
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
            
            Entities.WithAll<LinearMoveTag, CardCurrentMoveData>().WithNone<CardMoveProcess>().ForEach((Entity e, int entityInQueryIndex, CardCurrentMoveData current, TargetMoveData target) =>
            {
                var distance = math.distance(target.TargetMove, current.currentPosition);
                
                var linearData = new CardMoveProcess();
                var item = GetNextPosition(current.currentPosition, current.currentLocalScale, target.TargetMove, distance);
                linearData.nextPosition = item.Item1;
                linearData.nextScale = item.Item2;

                ecb.AddComponent(entityInQueryIndex, e, linearData);
                ecb.AddComponent(entityInQueryIndex, e, new LinearMoveData(){accumulatedTime = time, initDistanceToTarget = distance});
                
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
                         var next = moveData;
                         var item = GetNextPosition(current.currentPosition, current.currentLocalScale, target.TargetMove, linearData.initDistanceToTarget);
                         next.nextPosition = item.Item1;
                         next.nextScale = item.Item2;

                         linearData.accumulatedTime += time;
                         
                         ecb.SetComponent(entityInQueryIndex, e, next);
                         ecb.SetComponent(entityInQueryIndex, e, linearData);
                     }
                }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
        
        private static (float2, float2) GetNextPosition(float2 currentPosition, float2 currentScale, float2 target, float initDistance)
        {
            var distance = math.distance(target, currentPosition);

            var checkSpeed = math.clamp(distance / 10, 20, 100);

            var normalize = math.normalize(target - currentPosition);

            if (distance <= 10)
            {
                return (currentPosition + normalize * distance / 3, GetNextScale(distance, 0.1f, initDistance));
            }
            
            if (distance <= 3)
            {
                return (currentPosition + normalize * distance, GetNextScale(distance, 0.1f, initDistance));
            }
           
            return (normalize * checkSpeed + currentPosition, GetNextScale(distance, 0.1f, initDistance));
            
            float2 GetNextScale(float currentDistance, float minDistance, float startDistance)
            {
                var x = math.clamp(startDistance * minDistance / currentDistance, 0, 1);

                return new Vector2(x, x);
            }
        }
    }
}