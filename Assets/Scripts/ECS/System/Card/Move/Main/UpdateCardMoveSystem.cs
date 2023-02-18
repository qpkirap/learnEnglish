using Game.ECS_UI.Components;
using Game.ECS.Components;
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
                        CurrentPosition = card.Instance.Root.anchoredPosition,
                        CurrentLocalScale = (Vector2)card.Instance.Root.localScale
                    };
                    
                    EntityManager.AddComponentData(e, moveData);   
                    
                }).WithStructuralChanges().WithoutBurst().Run();
            
            Entities.WithAll<CardCurrentMoveData, InstanceTag, UICardControllerComponent>().ForEach(
                (Entity e, UICardControllerComponent card, ref CardCurrentMoveData move) =>
                {
                    move.CurrentPosition = card.Instance.Root.anchoredPosition;
                    move.CurrentLocalScale = (Vector2)card.Instance.Root.localScale;
                }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}