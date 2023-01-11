using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components;
using CraftCar.ECS.Components.SpawnData;
using CraftCar.ECS.Components.Tags;
using CraftCar.InitGame.ECS.Config;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Screen;
using Random = UnityEngine.Random;

namespace CraftCar.ECS.System.SpawnCard
{
    [AlwaysUpdateSystem]
    public partial class SpawnCardSystem : SystemBase
    {
        private UICanvasController canvas;
        
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

            var screenCenter = GetRandomPositionOutScreen();

            cardController.Root.anchoredPosition = screenCenter;
            
            cardController.gameObject.SetActive(true);

            EntityManager.AddComponentData(factorysEntity,
                new TargetMoveData()
                    { TargetMove = new float2(canvas.root.sizeDelta.x / 2, canvas.root.sizeDelta.y / 2) });
            EntityManager.AddComponentData(factorysEntity, new LinearMoveTag());
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

        private Vector2 GetRandomPositionOutScreen()
        {
            var canvasInstance = GetCanvas();

            var screenSize = canvasInstance.root.sizeDelta;

            var y = Random.Range(-screenSize.y / 2, screenSize.y * 2);

            var x = 0f;
            
            if (y >= 0 && y <= screenSize.y)
            {
                x = Random.Range(Random.Range(-screenSize.x / 2, 0), Random.Range(screenSize.x, screenSize.x * 2));
            }
            else x = Random.Range(-screenSize.x / 2, screenSize.x * 2);
            
            return new Vector2(x, y);
        }
    }
}