using Game.Ads;
using Game.ECS_UI.Components;
using Game.ECS_UI.Components.AdsCanvas;
using Game.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace Game.ECS.System
{
    public partial class CanvasSystem : UpdateSystem
    {
        private UICanvasController canvas;
        private Entity canvasE;

        private static LazyInject<GameState> gameState = new();

        protected override void OnUpdate()
        {
            if (GetCanvas(out var e) == null) return;

            //Клик по лидерам
            Entities.WithAll<LeaderBoardClickTag>().ForEach(() =>
            {
                var canvas = GetCanvas(out var cE);
                
                Debug.Log($"canvas {canvas}");
                
                if (!HasSingleton<AdsController>()) return;

                canvas?.adsCanvas.Activate(cE, EntityManager);

                EntityManager.RemoveComponent<LeaderBoardClickTag>(e);
                
            }).WithStructuralChanges().Run();
            
            //клик по продолжить регистарцию
            Entities.WithAll<LeaderBoardTryRegistrationTag>().ForEach((Entity e) =>
            {
                if (HasSingleton<FirebaseRegNotNowTag>())
                {
                    var entityNotNow = GetSingletonEntity<FirebaseRegNotNowTag>();

                    EntityManager.RemoveComponent<FirebaseRegNotNowTag>(entityNotNow);
                    EntityManager.RemoveComponent<LeaderBoardTryRegistrationTag>(e);
                }
            }).WithStructuralChanges().Run();
            
            //если нажали закрыть рекламный канвас
            Entities.WithAll<CloseAdsCanvasTag>().ForEach((Entity e) =>
            {
                EntityManager.RemoveComponent<CloseAdsCanvasTag>(e);

            }).WithStructuralChanges().Run();
            
            //нажали просмотреть рекламу
            Entities.WithAll<TryAdsViewTag>().ForEach((Entity e) =>
            {
                if (!HasSingleton<AdsController>()) return;

                var adsE = GetSingletonEntity<AdsController>();
                var ads = EntityManager.GetComponentData<AdsController>(adsE);
                
                var c = GetCanvas(out var cE);
                
                ads.InjectCanvas(cE, EntityManager);

                ads.ShowRewarded();

                c?.adsCanvas.Disable();

                EntityManager.RemoveComponent<TryAdsViewTag>(e);
                
                Debug.Log($"Показали рекламу");

            }).WithStructuralChanges().Run();
            
            //просмотрели удачно рекламу. Показать таблицу лидеров
            Entities.WithAll<AdsCompletedTag>().ForEach((Entity e) =>
            {
                EntityManager.RemoveComponent<AdsCompletedTag>(e);

                var can = GetCanvas(out var ent);

                if (gameState.Value != null)
                {
                    can.LeaderCanvas.InjectActivation(gameState.Value.LeadersState.LeaderDatas);
                }
                
                Debug.Log($"Завершили показ рекламы is null game state {gameState.Value == null}!!");
                
            }).WithStructuralChanges().Run();
            
            //При просмотре рекламы ошибка. Удаляемся назад
            Entities.WithAll<AdsErrorTag>().ForEach((Entity e) =>
            {
                EntityManager.RemoveComponent<AdsErrorTag>(e);

                var can = GetCanvas(out var ent);
                
                Debug.Log($"Ошибка рекламы");
                
            }).WithStructuralChanges().Run();
        }
        
        private UICanvasController GetCanvas(out Entity entity)
        {
            entity = canvasE;
            
            if (canvas != null) return this.canvas;

            Entities.WithAll<UICanvasController>().ForEach((Entity e, in UICanvasController canvasController) =>
            {
                canvasE = e;
                
                canvas = canvasController;
            }).WithoutBurst().Run();

            return canvas;
        }
    }
}