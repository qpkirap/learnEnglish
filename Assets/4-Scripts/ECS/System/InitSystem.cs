using Game.CustomPool;
using Game.ECS.Components;
using JohanPolosn.UnityInjector;
using UniRx;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

namespace Game.ECS.System
{
    public partial class InitSystem : InitSystemBase
    {
        public static ReactiveProperty<bool> IsInitGameState = new();

        private GameState gameState;
        private CompositionRoot root;
        
        protected override void OnStartRunning()
        {
            Application.targetFrameRate = 60;
            
            root = DInjector.CreateCompositionRoot(DITag.game);

            var factorySingleton = GetSingletonEntity<FactoriesCardData>();
            var factoryData = EntityManager.GetComponentData<FactoriesCardData>(factorySingleton);

            SetSleepTimeoutState(true);
            CreateGameState();
            factoryData.InitAllFabrics();
        }

        private void CreateGameState()
        {
            gameState = new GameState();
            
            gameState.LoadSave();
            
            IsInitGameState.Value = true;
        }

        protected override void OnUpdate()
        {
            if (!HasSingleton<InitAllFabricsTag>() && IsLoadFabrics() && PoolManager.Instance != null)
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
        
        private void SetSleepTimeoutState(bool state)
        {
            Screen.sleepTimeout = state
                ? SleepTimeout.NeverSleep
                : SleepTimeout.SystemSetting;
        }
    }
}