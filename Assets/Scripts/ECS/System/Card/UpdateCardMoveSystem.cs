using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace CraftCar.ECS.System.Card
{
    [AlwaysUpdateSystem]
    public partial class UpdateCardMoveSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer();
            
            Entities.WithAll<CardTag, InstanceTag, UICardControllerComponent>().ForEach(
                (Entity e, UICardControllerComponent card) =>
                {
                    var moveData = new CardCurrentMoveData()
                    {
                        currentPosition = card.Instance.Root.anchoredPosition,
                        currentLocalScale = (Vector2)card.Instance.Root.localScale
                    };
                    
                    ecb.AddComponent(e, moveData);   
                    
                }).WithoutBurst().Run();
        }
    }
}