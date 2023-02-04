using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components;
using Game.ECS.System.Base;
using Unity.Entities;
using UnityEngine;

namespace Game.ECS.System
{
    public partial class UpdateCardMoveSystem : PreMovementSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<CardTag, InstanceTag, UICardControllerComponent>().WithNone<CardCurrentMoveData>().ForEach(
                (Entity e, UICardControllerComponent card) =>
                {
                    var moveData = new CardCurrentMoveData()
                    {
                        currentPosition = card.Instance.Root.anchoredPosition,
                        currentLocalScale = (Vector2)card.Instance.Root.localScale
                    };
                    
                    EntityManager.AddComponentData(e, moveData);   
                    
                }).WithStructuralChanges().WithoutBurst().Run();
            
            Entities.WithAll<CardCurrentMoveData, InstanceTag, UICardControllerComponent>().ForEach(
                (Entity e, UICardControllerComponent card, ref CardCurrentMoveData move) =>
                {
                    move.currentPosition = card.Instance.Root.anchoredPosition;
                    move.currentLocalScale = (Vector2)card.Instance.Root.localScale;
                }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}