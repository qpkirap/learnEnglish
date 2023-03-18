using Game.ECS.Components;
using Unity.Entities;

namespace Game.ECS.System
{
    [UpdateBefore(typeof(NextCardSystem))]
    public partial class RatingUpgradePointClickSystem : UpdateSystem
    {
        private static GameState gameState;
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!HasSingleton<GameState>()) return;

            if (gameState == null)
            {
                var gameStateEntity = GetSingletonEntity<GameState>();
                gameState = EntityManager.GetComponentData<GameState>(gameStateEntity);
            }
            
            if (gameState == null) return;

            Entities.WithAll<ClickNextButtonTag, InstanceTag>().WithNone<RatingPointClickTag>().ForEach(() =>
            {
                gameState.UserState.UpgradePointClick();
            }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}