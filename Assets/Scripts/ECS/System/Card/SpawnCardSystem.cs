using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components;
using CraftCar.ECS.Components.SpawnData;
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
            if (!HasSingleton<FactoriesCardData>()) return;
            if (!HasSingleton<EntityDicElementsData>()) return;
            
            int countCard = 0;

            Entities.WithAll<CardTag>().ForEach((Entity e) =>
            {
                countCard++;
            }).WithoutBurst().Run();

            if (countCard == 0)
            {
                CreateCard();
            }
            
        }

        private void CreateCard()
        {
            var canvas = GetCanvas();
                
            if (canvas == null) return;

            var dicData = GetSingleton<EntityDicElementsData>();
                
            var cardEntity = EntityManager.CreateEntity(typeof(CardTag));
            EntityManager.AddComponentData(cardEntity, new CardTag());
                
            var factorysEntity = GetSingletonEntity<FactoriesCardData>();
            var factories = EntityManager.GetComponentData<FactoriesCardData>(factorysEntity);

            var randomData = dicData.GetRandomData();

            factories.CreateCardInstance<TestCardMono>(cardEntity, canvas.root);

            EntityManager.AddComponentData(cardEntity, randomData);
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