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
                var linearData = new CardMoveProcess();
                linearData.nextPosition = GetNextPosition(current.currentPosition, target.TargetMove);
                linearData.nextScale = Vector2.one;
                
                ecb.AddComponent(entityInQueryIndex, e, linearData);
                
            }).ScheduleParallel();

            Entities.WithAll<LinearMoveTag, CardMoveProcess, CardCurrentMoveData>().ForEach(
                (Entity e, int entityInQueryIndex, CardCurrentMoveData current, TargetMoveData target, ref CardMoveProcess linearMoveData) =>
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
                         var next = linearMoveData;
                         next.nextPosition = linearMoveData.nextPosition = GetNextPosition(current.currentPosition, target.TargetMove);
                         ecb.SetComponent(entityInQueryIndex, e, next);
                     }
                }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
        
        public static float2 GetNextPosition(float2 currentPosition, float2 target)
        {
            var distance = math.distance(target, currentPosition);
            
            var checkSpeed = math.clamp(distance / 10, 20, 100);

            var normalize = math.normalize(target - currentPosition);

            if (distance <= 10)
            {
                return currentPosition + normalize * distance / 3;
            }
            
            if (distance <= 3)
            {
                return currentPosition + normalize * distance;
            }
           
            return normalize * checkSpeed + currentPosition;
        }
        
        
    }
}