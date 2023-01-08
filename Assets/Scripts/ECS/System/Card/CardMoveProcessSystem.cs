using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace CraftCar.ECS.System.Card
{
    public partial class CardMoveProcessSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<UICardControllerComponent, CardMoveProcess>().ForEach((Entity e, CardMoveProcess move, UICardControllerComponent card) =>
            {
                // card.Instance.Root.anchoredPosition = move.nextPosition;
                // card.Instance.Root.localScale = new Vector2(move.nextScale.x, move.nextScale.y);
            }).WithoutBurst().Run();
        }
    }
}