using System.Collections.Generic;
using CraftCar.InitGame.ECS.Components.Scene;
using CraftCar.InitGame.GameResources;
using CraftCar.InitGame.GameResources.Base;
using Unity.Collections;
using Unity.Entities;

namespace CraftCar.ECS.System
{
    //[UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
    public partial class InitSystem : SystemBase
    {
        private List<CreateEntityObjectsFactory> tempLoadFabrics;
        
        private bool isLoadAllFabrics;

        protected override void OnStartRunning()
        {
            var factorysEntity = GetSingletonEntity<FactoriesComponentData>();
            var factorysData = EntityManager.GetComponentData<FactoriesComponentData>(factorysEntity);
            var cardFactory = factorysData.GetFabric<SimpleViewLearnFactory>();

            tempLoadFabrics = new List<CreateEntityObjectsFactory>(factorysData.UiCardFabrics.Count);

            cardFactory.Init();
            
            tempLoadFabrics.Add(cardFactory);
        }

        protected override void OnUpdate()
        {
            if (isLoadAllFabrics || tempLoadFabrics == null) return;

            foreach (var entityObjectsFactory in tempLoadFabrics)
            {
                if (!entityObjectsFactory.IsLoadPrefab) return;
            }

            isLoadAllFabrics = true;
        }

        // private void LoadGameScene()
        // {
        //     
        //     var gameSceneEntity = GetSingletonEntity<GameScene>();
        //     var gameScene = EntityManager.GetComponentData<SceneReference>(gameSceneEntity);
        //     
        //     sceneSystem.LoadSceneAsync(gameScene.SceneGUID,
        //         new SceneSystem.LoadParameters {AutoLoad = true, Flags = SceneLoadFlags.LoadAdditive});
        //     
        // }
    }
    
    
}