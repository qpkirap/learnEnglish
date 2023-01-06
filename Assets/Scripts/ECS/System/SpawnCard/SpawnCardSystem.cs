using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components;
using CraftCar.InitGame.ECS.Config;
using Unity.Entities;
using UnityEngine;

namespace CraftCar.ECS.System.SpawnCard
{
    [AlwaysUpdateSystem]
    public partial class SpawnCardSystem : SystemBase
    {
        protected override void OnCreate()
        {
            Debug.Log("SpawnCardSystem create");
        }
        
        protected override void OnUpdate()
        {
            if (!HasSingleton<FactoriesCardData>())
            {
                return;
            }
            
            int countCard = 0;

            Entities.WithAll<CardTag>().ForEach((Entity e) =>
            {
                countCard++;
            }).WithoutBurst().Run();

            if (countCard == 0)
            {
                var canvas = GetCanvas();
                
                if (canvas == null) return;
                
                var cardEntity = EntityManager.CreateEntity(typeof(CardTag));
                EntityManager.AddComponentData(cardEntity, new CardTag());
                
                var factorysEntity = GetSingletonEntity<FactoriesCardData>();
                var factories = EntityManager.GetComponentData<FactoriesCardData>(factorysEntity);

                

                factories.CreateCardInstance<TestCardMono>(cardEntity, canvas.root);
            }
            
        }

        private UICanvasController GetCanvas()
        {
            UICanvasController canvas = null;
            
            Entities.WithAll<UICanvasController>().ForEach((Entity e, in UICanvasController canvasController) =>
            {
                canvas = canvasController;
            }).WithoutBurst().Run();

            return canvas;
        }
    }
}