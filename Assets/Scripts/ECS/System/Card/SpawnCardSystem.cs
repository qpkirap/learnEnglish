using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components;
using CraftCar.ECS.Components.SpawnData;
using CraftCar.InitGame.ECS.Config;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Screen;

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

            var cardController = factories.CreateCardInstance<TestCardMono>(cardEntity, canvas.root);

            EntityManager.AddComponentData(cardEntity, randomData);
            
            cardController.Inject(cardEntity, EntityManager);

            var screenCenter = new float2(canvas.root.sizeDelta.x / 2, canvas.root.sizeDelta.y / 2);

            cardController.Root.anchoredPosition = screenCenter;
            
            cardController.gameObject.SetActive(true);
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