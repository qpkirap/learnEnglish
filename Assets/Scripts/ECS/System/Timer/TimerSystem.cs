using CraftCar.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CraftCar.ECS.System
{
    public partial class TimerSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            Debug.Log("TimerSystem create");
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities.WithAll<Timer>().ForEach((Entity e, int entityInQueryIndex, ref Timer timer) =>
            {
                if (timer.IsPaused)
                {
                }
                else
                {
                    var newTimer = new Timer
                    {
                        TimeLeft = math.max(timer.TimeLeft - Time.DeltaTime * timer.Timescale, 0),
                        Timescale = timer.Timescale,
                        TotalTime = timer.TotalTime,
                        IsPaused = timer.IsPaused,
                        IsScalable = timer.IsScalable
                    };

                    ecb.SetComponent(entityInQueryIndex, e, newTimer);
                }
            }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}