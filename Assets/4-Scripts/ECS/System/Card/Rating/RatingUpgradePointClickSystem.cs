﻿using Game.ECS_UI.Components;
using Game.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace Game.ECS.System
{
    [UpdateAfter(typeof(FirebaseLeaderPointClickUpdateSystem))]
    public partial class RatingUpgradePointClickSystem : UpdateSystem
    {
        private static LazyInject<GameState> gameState = new();
        private readonly LazyInject<UICanvasController> canvas = new();
        
        protected override void OnUpdate()
        {
            if (gameState.Value == null) return;
            
            Entities.WithAll<ClickNextButtonTag, InstanceTag>().WithNone<RatingPointClickTag>().ForEach((Entity e) =>
            {
                gameState.Value.UserState.UpgradePointClick();

                EntityManager.AddComponentData(e, new RatingPointClickTag());
                
                Debug.Log("UpdatePointClick");
                
            }).WithStructuralChanges().WithoutBurst().Run();

            var canvas = GetCanvas();
            
            if (canvas == null) return;
            
            canvas.LeaderBoard.currentClickPoint.text = gameState.Value.UserState.PointClick.ToString();
            canvas.LeaderBoard.currentNick.text = gameState.Value.UserState.Nick ?? string.Empty;
        }
        
        private UICanvasController GetCanvas()
        {
            return canvas.Value;
        }
    }
}