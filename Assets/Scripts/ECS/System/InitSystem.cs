using CraftCar.ECS.Components.Tags;
using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

namespace CraftCar.ECS.System
{
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class InitSystem : SystemBase
    {
        private SceneSystem sceneSystem;
        
        private Entity gameSceneEntity;

        private NativeArray<Entity> fabricsCard;

        private bool IsInit { get; set; }

        protected override void OnStartRunning()
        {
            Application.targetFrameRate = 60;
            
            sceneSystem = World.GetExistingSystem<SceneSystem>();
            gameSceneEntity = GetSingletonEntity<SceneReference>();
            
            var factorySingleton = GetSingletonEntity<FactoriesCardData>();
            var factoryData = EntityManager.GetComponentData<FactoriesCardData>(factorySingleton);

            fabricsCard = factoryData.InitAllFabrics();
        }

        protected override void OnUpdate()
        {
            if (IsInit) return;

            if (TryInitCardFabrics())
            {
                if (!sceneSystem.IsSceneLoaded(gameSceneEntity))
                {
                    LoadGameScene();

                    IsInit = true;
                }
            }
        }

        private bool TryInitCardFabrics()
        {
            if (fabricsCard == null || !fabricsCard.IsCreated) return false;
            
            foreach (var entity in fabricsCard)
            {
                if(!EntityManager.HasComponent<ReadyPrefabTag>(entity)) return false;
            }

            fabricsCard.Dispose();

            return true;
        }

        private void LoadGameScene()
        {
            var gameSceneEntityData = EntityManager.GetComponentData<SceneReference>(gameSceneEntity);

            sceneSystem.LoadSceneAsync(gameSceneEntityData.SceneGUID,
                new SceneSystem.LoadParameters {AutoLoad = false, Flags = SceneLoadFlags.LoadAsGOScene });
        }
    }
}