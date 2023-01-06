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
            if (!HasSingleton<UICanvasAuthoring>())
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
                var cardEntity = EntityManager.CreateEntity(typeof(CardTag));
                EntityManager.AddComponentData(cardEntity, new CardTag());
                
                var factorysEntity = GetSingletonEntity<FactoriesCardData>();
                var factories = EntityManager.GetComponentData<FactoriesCardData>(factorysEntity);

                var canvasEntity = GetSingletonEntity<Canvas>();
                var canvasData = EntityManager.GetComponentData<UICanvasAuthoring>(canvasEntity);

                factories.CreateCardInstance<TestCardMono>(cardEntity);
            }
            
        }
    }
}