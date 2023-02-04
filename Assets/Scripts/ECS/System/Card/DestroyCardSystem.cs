using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components.Tags;
using Game.ECS.System.Base;
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

            Entities.WithAll<DestroyTag>()
                .ForEach((Entity e, UICardControllerComponent card) =>
                {
                    PoolManager.Instance.ReleaseObject(card.Instance.gameObject);

                    ecb.DestroyEntity(e);
                }).WithoutBurst().Run();
        }
    }
}