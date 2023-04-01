using Game.ECS_UI.Components;
using Game.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace Game.ECS.System
{
    [UpdateAfter(typeof(FirebaseLeaderPointClickUpdateSystem))]
    public partial class RatingUpgradePointClickSystem : UpdateSystem
    {
        private UICanvasController canvas;
        
        protected override void OnUpdate()
        {
            if (!HasSingleton<GameState>()) return;
            
            Entities.WithAll<ClickNextButtonTag, InstanceTag>().WithNone<RatingPointClickTag>().ForEach((Entity e) =>
            {
                var gameStateEntity = GetSingletonEntity<GameState>();
                var gameState = EntityManager.GetComponentData<GameState>(gameStateEntity);
                
                gameState.UserState.UpgradePointClick();

                EntityManager.AddComponentData(e, new RatingPointClickTag());
                
                Debug.Log("UpdatePointClick");
                
            }).WithStructuralChanges().WithoutBurst().Run();

            var canvas = GetCanvas();
            
            if (canvas == null) return;
            
            Entities.WithAll<GameState>().ForEach((GameState gameState) =>
            {
                canvas.LeaderBoard.currentClickPoint.text = gameState.UserState.PointClick.ToString();
                canvas.LeaderBoard.currentNick.text = gameState.UserState.Nick ?? string.Empty;
            }).WithoutBurst().Run();
        }
        
        private UICanvasController GetCanvas()
        {
            if (canvas != null) return this.canvas;

            Entities.WithAll<UICanvasController>().ForEach((Entity e, in UICanvasController canvasController) =>
            {
                canvas = canvasController;
            }).WithoutBurst().Run();

            return canvas;
        }
    }
}