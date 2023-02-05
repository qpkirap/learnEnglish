using CraftCar;
using CraftCar.ECS.Components;
using Game.ECS.Components;
using Game.ECS.System.Base;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

namespace Game.ECS.System
{
    public partial class InitSystem : InitSystemBase
    {
        protected override void OnStartRunning()
        {
            Application.targetFrameRate = 60;

            var factorySingleton = GetSingletonEntity<FactoriesCardData>();
            var factoryData = EntityManager.GetComponentData<FactoriesCardData>(factorySingleton);

            factoryData.InitAllFabrics();
        }

        protected override void OnUpdate()
        {
            if (!HasSingleton<InitAllFabricsTag>() && IsLoadFabrics())
            {
                Entities.WithAll<FactoriesCardData>().WithNone<InitAllFabricsTag>().ForEach((ref Entity e) =>
                {
                    EntityManager.AddComponentData(e, new InitAllFabricsTag());

                    LoadGameScene();
                }).WithStructuralChanges().WithoutBurst().Run();
            }
        }

        private bool IsLoadFabrics()
        {
            var test = 0;

            Entities.WithAll<Prefab>().WithNone<ReadyPrefabTag>().ForEach((Entity e) => { test--; }).WithoutBurst()
                .Run();

            return test == 0;
        }

        private void LoadGameScene()
        {
            var sceneSystem = World.GetExistingSystem<SceneSystem>();
            var gameSceneEntity = GetSingletonEntity<SceneReference>();

            var gameSceneEntityData = EntityManager.GetComponentData<SceneReference>(gameSceneEntity);

            sceneSystem.LoadSceneAsync(gameSceneEntityData.SceneGUID,
                new SceneSystem.LoadParameters { AutoLoad = false, Flags = SceneLoadFlags.LoadAsGOScene });
        }
    }
}