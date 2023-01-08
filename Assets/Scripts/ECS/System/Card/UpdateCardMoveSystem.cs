using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace CraftCar.ECS.System.Card
{
    public partial class UpdateCardMoveSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<CardTag, InstanceTag, UICardControllerComponent>().ForEach(
                (Entity e, UICardControllerComponent card) =>
                {
                    var moveData = new CardMoveData()
                    {
                        currentPosition = card.Instance.Root.anchoredPosition,
                        currentLocalScale = (Vector2)card.Instance.Root.localScale
                    };
                    
                    EntityManager.AddComponentData(e, moveData);   
                    
                }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}