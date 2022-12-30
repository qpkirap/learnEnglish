using CraftCar.ECS_UI.Components;
using CraftCar.ECS_UI.Components.Canvas;
using CraftCar.ECS.Components.Tags;
using CraftCar.InitGame.ECS.Config;
using Unity.Entities;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;

namespace CraftCar.ECS.System
{
    //[UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
    public partial class InitSystem : SystemBase
    {
        private Entity testEntity;
        private bool isLoadSceneStart;
        private bool isTest;
        private SceneSystem sceneSystem;
        private Entity gameSceneEntity;

        protected override void OnStartRunning()
        {
            var factorysEntity = GetSingletonEntity<FactoriesComponentData>();
            var factorysData = EntityManager.GetComponentData<FactoriesComponentData>(factorysEntity);
            
            sceneSystem = World.GetExistingSystem<SceneSystem>();
            
            testEntity = factorysData.GetPrefab<TestEntitySharedConfig>();
        }

        protected override void OnUpdate()
        {
            if (!isLoadSceneStart && EntityManager.HasComponent<ReadyPrefabTag>(testEntity))
            {
                LoadGameScene();
            }

            if (isLoadSceneStart && !sceneSystem.IsSceneLoaded(gameSceneEntity) && !isTest)
            {
                var data = EntityManager.GetSharedComponentData<UICardControllerComponent>(testEntity);

                var convert = GameObjectConversionUtility.ConvertGameObjectHierarchy(data.uiCardController.gameObject,
                    GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld,
                        new BlobAssetStore()));

                EntityManager.Instantiate(convert);
                EntityManager.Instantiate(convert);
                EntityManager.Instantiate(convert);
                EntityManager.Instantiate(convert);

                Entities.ForEach((Entity e, in UICanvasController canvasController) =>
                {
                    
                }).WithStructuralChanges().WithoutBurst().Run();
            }
        }

        private void LoadGameScene()
        {
            gameSceneEntity = GetSingletonEntity<SceneReference>();
            
            var gameSceneEntityData = EntityManager.GetComponentData<SceneReference>(gameSceneEntity);

            sceneSystem.LoadSceneAsync(gameSceneEntityData.SceneGUID,
                new SceneSystem.LoadParameters {AutoLoad = false, Flags = SceneLoadFlags.LoadAsGOScene });
            
            isLoadSceneStart = true;
        }
    }
    
    
}