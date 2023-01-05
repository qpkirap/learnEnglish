using CraftCar.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace CraftCar.ECS.System
{
    public partial class TimerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            
            Entities.WithAll<Timer>().ForEach((Entity e, int entityInQueryIndex, ref Timer timer) =>
            {
                if (timer.IsPaused)
                {
                }
                else
                {
                    timer.TimeLeft = math.max(timer.TimeLeft - deltaTime * timer.Timescale, 0);
                }
            }).ScheduleParallel();
        }
    }
}