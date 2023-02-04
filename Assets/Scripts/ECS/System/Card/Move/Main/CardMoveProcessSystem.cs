using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components;
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
                card.Instance.Root.anchoredPosition = move.nextPosition;
                card.Instance.Root.localScale = new Vector2(move.nextScale.x, move.nextScale.y);
            }).WithoutBurst().Run();
        }
    }
}