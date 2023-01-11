using CraftCar.ECS.Components;
using CraftCar.ECS.Components.Tags;
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
            
            Entities.WithAll<LinearMoveTag, CardCurrentMoveData>().WithNone<LinearMoveData>().ForEach((Entity e, int entityInQueryIndex, CardCurrentMoveData current, TargetMoveData target) =>
            {
                var linearData = new CardMoveProcess();
                linearData.nextPosition = GetNextPosition(current.currentPosition, target.TargetMove);
                linearData.nextScale = Vector2.one;
                
                ecb.AddComponent(entityInQueryIndex, e, linearData);
                
            }).ScheduleParallel();

            Entities.WithAll<LinearMoveTag, LinearMoveData, CardCurrentMoveData>().ForEach(
                (Entity e, int entityInQueryIndex, CardCurrentMoveData current, TargetMoveData target, ref CardMoveProcess linearMoveData) =>
                {
                     if (math.distance(target.TargetMove, current.currentPosition) <= 0.00001)
                     {
                         ecb.RemoveComponent<CardMoveProcess>(entityInQueryIndex, e);
                         ecb.RemoveComponent<LinearMoveData>(entityInQueryIndex, e);
                         ecb.RemoveComponent<LinearMoveTag>(entityInQueryIndex, e);
                     }
                      else linearMoveData.nextPosition = GetNextPosition(current.currentPosition, target.TargetMove);
                }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }

        public static float2 GetNextPosition(float2 currentPosition, float2 target)
        {
            float speed = 1;

            return math.normalize(target - currentPosition) * speed;
        }
        
        
    }
}