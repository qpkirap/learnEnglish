using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components.Tags;
using Game.CustomPool;
using Unity.Entities;
using UnityEngine.PlayerLoop;

namespace CraftCar.ECS.System.Card
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class CardDestroySystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

            Entities.WithAll<UICardControllerComponent, DestroyTag>()
                .ForEach((Entity e, UICardControllerComponent card) =>
                {
                    PoolManager.Instance.ReleaseObject(card.Instance.gameObject);
                    
                    ecb.DestroyEntity(e);
                }).WithoutBurst().Run();
        }
    }
}