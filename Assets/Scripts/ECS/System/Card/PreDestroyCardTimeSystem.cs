using Game.ECS.Components;
using Unity.Entities;

namespace Game.ECS.System
{
    public partial class PreDestroyCardTimeSystem : PreDestroySystem
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        private const bool IsScalable = false;
        private const bool IsPaused = false;
        private const float TotalTime = 0.4f;
        private const float TimeLeft = 0.4f;
        private const float Timescale = 1f;
        
        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities.WithAll<PreDestroyTag>().WithNone<Timer>()
                .ForEach((Entity e, int entityInQueryIndex) =>
                {
                    var timer = new Timer()
                    {
                        IsScalable = IsScalable,
                        IsPaused = IsPaused,
                        TotalTime = TotalTime,
                        TimeLeft = TimeLeft,
                        Timescale = Timescale
                    };

                    ecb.AddComponent(entityInQueryIndex, e, timer);
                }).ScheduleParallel();
            
            Entities.WithAll<Timer, PreDestroyTag>().WithNone<DestroyTag>()
                .ForEach((Entity e, int entityInQueryIndex, Timer timer) =>
            {
                if (timer.IsCompleted) ecb.AddComponent(entityInQueryIndex, e, new DestroyTag());
            }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}