using Game.ECS_UI.Components;
using Game.ECS.Components;
using Game.ECS.System.Base;
using Unity.Entities;
using UnityEngine;

namespace Game.ECS.System
{
    public partial class CardMoveProcessSystem : PostMovementSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<UICardControllerComponent, CardMoveProcess>().ForEach((Entity e, CardMoveProcess move, UICardControllerComponent card) =>
            {
                card.Instance.Root.anchoredPosition = move.NextPosition;
                card.Instance.Root.localScale = new Vector2(move.NextScale.x, move.NextScale.y);
            }).WithoutBurst().Run();
        }
    }
}