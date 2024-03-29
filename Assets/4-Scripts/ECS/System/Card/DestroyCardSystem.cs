using Game.ECS_UI.Components;
using Game.ECS.Components;
using Game.CustomPool;
using Unity.Entities;

namespace Game.ECS.System
{
    public partial class DestroyCardSystem : DestroySystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

            Entities.WithAll<DestroyTag, CardTag>()
                .ForEach((Entity e, UICardControllerComponent card) =>
                {
                    PoolManager.Instance.ReleaseObject(card.Instance.gameObject);

                    ecb.DestroyEntity(e);
                }).WithoutBurst().Run();
        }
    }
}