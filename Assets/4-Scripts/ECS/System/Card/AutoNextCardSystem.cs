using Game.ECS.Components;
using Unity.Entities;

namespace Game.ECS.System
{
    public partial class AutoNextCardSystem : UpdateSystem
    {
        private const bool IsScalable = false;
        private const bool IsPaused = false;
        private const float TimeLeft = 1f;
        private const float Timescale = 1f;
        
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private static readonly LazyInject<GameState> gameState = new();

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            if (gameState.Value == null) return;
            if (!gameState.Value.SettingsState.IsAutoNextCard) return;
            
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            var isActiveEngSpeech = gameState.Value.SettingsState.IsActiveEngSpeech;
            var isActiveRusSpeech = gameState.Value.SettingsState.IsActiveRusSpeech;

            Entities
                .WithAll<CardTag, InstanceTag>()
                .WithNone<PreDestroyTag, CardMoveProcess, AutoNextCardTag>()
                .ForEach((Entity e, int entityInQueryIndex) =>
                {
                    var timer = new Timer()
                    {
                        IsScalable = false,
                        IsPaused = false,
                        TimeLeft = TimeLeft,
                        Timescale = Timescale
                    };
                    
                    ecb.AddComponent(entityInQueryIndex, e, timer);
                    ecb.AddComponent<AutoNextCardTag>(entityInQueryIndex, e);
                }).ScheduleParallel();
            
            Entities
                .WithAll<CardTag, AutoNextCardTag, Timer>()
                .WithNone<CardMoveProcess, ClickNextButtonTag, SpeechAllCompleteTag>()
                .ForEach((Entity e, int entityInQueryIndex, Timer timer) =>
                {
                    if (timer.IsCompleted 
                        && (!isActiveEngSpeech
                        && !isActiveRusSpeech))
                    {
                        ecb.AddComponent(entityInQueryIndex, e, new ClickNextButtonTag());
                        ecb.RemoveComponent<Timer>(entityInQueryIndex, e);
                    }
                }).ScheduleParallel();
            
            Entities
                .WithAll<CardTag, AutoNextCardTag, SpeechAllCompleteTag>()
                .WithNone<CardMoveProcess, ClickNextButtonTag>()
                .ForEach((Entity e, int entityInQueryIndex) =>
                {
                    ecb.AddComponent(entityInQueryIndex, e, new ClickNextButtonTag());
                }).ScheduleParallel();

            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }

    public struct AutoNextCardTag : IComponentData
    {
    }
}