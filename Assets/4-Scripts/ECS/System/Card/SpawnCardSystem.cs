using Game.ECS_UI.Components;
using Game.ECS.Components;
using Game.Config;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.ECS.System.SpawnCard
{
    public partial class SpawnCardSystem : UpdateSystem
    {
        private UICanvasController canvas;
        private const float offsetSenterY = 107;

        protected override void OnCreate()
        {
            Debug.Log("SpawnCardSystem create");
        }

        protected override void OnUpdate()
        {
            if (!HasSingleton<InitAllFabricsTag>()) return;
            if (!HasSingleton<EntityDicElementsData>()) return;
            if (!HasSingleton<FirebaseAppReadyTag>()) return;

            int countCard = 0;

            Entities.WithAll<CardTag>().ForEach((Entity e) => { countCard++; }).WithoutBurst().Run();

            if (countCard == 0)
            {
                var canvas = GetCanvas();

                if (canvas == null) return;

                Entities.WithAll<FactoriesCardData>().ForEach((FactoriesCardData factories) =>
                {
                    var dicData = GetSingleton<EntityDicElementsData>();

                    var randomWordsData = dicData.GetRandomData();

                    var cardEntity = EntityManager.CreateEntity(typeof(CardTag));
                    EntityManager.AddComponentData(cardEntity, new CardTag());

                    var cardController = factories.CreateCardInstance<SimpleCardFabric>(cardEntity, canvas.CardRoot);

                    cardController.Inject(cardEntity, EntityManager);

                    var screenCenter = GetRandomPositionOutScreen();

                    cardController.Root.anchoredPosition = screenCenter;
                    cardController.Root.localScale = Vector2.zero;

                    cardController.gameObject.SetActive(true);

                    var random = Random.Range(0f, 1f);

                    EntityManager.AddComponentData(cardEntity, randomWordsData);
                    EntityManager.AddComponentData(cardEntity,
                        new TargetMoveData()
                            { TargetMove = new float2(canvas.Root.rect.width / 2, canvas.Root.rect.height / 2 - offsetSenterY) });
                    EntityManager.AddComponentData(cardEntity, new LinearMoveTag());
                    EntityManager.AddComponentData(cardEntity, new RandomData() { Random = random > 0.5f });
                }).WithStructuralChanges().WithoutBurst().Run();
            }
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

            var screenSize = canvasInstance.CardRoot.sizeDelta;

            var y = Random.Range(-screenSize.y / 2, screenSize.y * 1.5f);

            var x = Random.Range(-screenSize.x / 2, screenSize.x * 1.5f);

            return new Vector2(x, y);
        }
    }
}