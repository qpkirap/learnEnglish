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
            cardController.Root.localScale = Vector2.zero;
            
            cardController.gameObject.SetActive(true);
            var random = Random.Range(0f, 1f);

            EntityManager.AddComponentData(cardEntity,
                new TargetMoveData()
                    { TargetMove = new float2(canvas.root.sizeDelta.x / 2, canvas.root.sizeDelta.y / 2) });
            EntityManager.AddComponentData(cardEntity, new LinearMoveTag());
            EntityManager.AddComponentData(cardEntity, new RandomData() { random = random > 0.5f });
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

            var y = Random.Range(-screenSize.y / 2, screenSize.y * 1.5f);
            
            var x = Random.Range(-screenSize.x / 2, screenSize.x * 1.5f);
            
            return new Vector2(x, y);
        }
    }
}