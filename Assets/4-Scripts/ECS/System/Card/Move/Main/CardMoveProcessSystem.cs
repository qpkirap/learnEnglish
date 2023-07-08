using DG.Tweening;
using Game.ECS_UI.Components;
using Game.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
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

                if (move.Width > 0 && math.abs(move.Width - card.BaseInstance.currentWidth) > .1f)
                {
                    var rootSizeDelta = card.Instance.Root.sizeDelta;
                    rootSizeDelta.x = move.Width;
                    rootSizeDelta.y = Screen.height;
 
                    //card.Instance.Root.DOSizeDelta(rootSizeDelta, 1f); 

                    var sequence = DOTween.Sequence(); 

                    sequence.Append(card.Instance.Root
                            .DOAnchorPos(new Vector2(Screen.width / 2, Screen.height / 2), .3f))
                        .Append(card.Instance.Root
                            .DOSizeDelta(rootSizeDelta, 0.3f));
                    card.BaseInstance.currentWidth = rootSizeDelta.x;

                    sequence.Play();
                }
            }).WithoutBurst().Run();
        }
    }
}