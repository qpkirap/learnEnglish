using CraftCar.ECS.Components;
using Game.ECS.System.Base;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.ECS.System
{
    public partial class TimerSystem : UpdateSystem
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