using Game.ECS_UI.Components;
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

            var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Entities.WithAll<ClickNextButtonTag, InstanceTag>().WithNone<RatingPointClickTag>().ForEach((Entity e, int entityInQueryIndex) =>
            {
                gameState.UserState.UpgradePointClick();
                
                ecb.AddComponent(entityInQueryIndex, e, new RatingPointClickTag());
            }).ScheduleParallel();
            
            Entities.WithAll<LeaderBoardController>().ForEach((LeaderBoardController controller) =>
            {
                controller.currentClickPoint.text = gameState.UserState.PointClick.ToString();
            }).WithoutBurst().Run();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}