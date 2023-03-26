using Game.ECS_UI.Components;
using Game.ECS.Components;
using Unity.Entities;

namespace Game.ECS.System
{
    [UpdateAfter(typeof(FirebaseLeaderPointClickUpdateSystem))]
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
            }).WithoutBurst().ScheduleParallel();
            
            if (!HasSingleton<LeaderBoardController>()) return;
            
            Entities.WithAll<LeaderBoardController>().ForEach((LeaderBoardController controller) =>
            {
                controller.currentClickPoint.text = gameState.UserState.PointClick.ToString();
                controller.currentNick.text = gameState.UserState.Nick ?? string.Empty;
            }).WithoutBurst().Run();
            
            //_entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}