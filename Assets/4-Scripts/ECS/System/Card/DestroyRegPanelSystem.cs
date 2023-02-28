using Game.CustomPool;
using Game.ECS_UI.Components;
using Game.ECS.Components;
using Unity.Entities;

namespace Game.ECS.System
{
    public partial class DestroyRegPanelSystem : DestroySystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

            Entities.WithAll<DestroyTag, UIRegistrationPanelComponent>()
                .ForEach((Entity e, UIRegistrationPanelComponent panel) =>
                {
                    PoolManager.Instance.ReleaseObject(panel.Instance.gameObject);

                    ecb.DestroyEntity(e);
                }).WithoutBurst().Run();
        }
    }
}