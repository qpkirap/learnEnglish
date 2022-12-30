using CraftCar.ECS.Components.Tags;
using CraftCar.InitGame.ECS.Config;
using Unity.Entities;
using Unity.Scenes;

namespace CraftCar.ECS.System
{
    //[UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
    public partial class InitSystem : SystemBase
    {
        private SceneSystem sceneSystem;
        
        private Entity testEntity;
        private Entity gameSceneEntity;
        
        public bool IsInit { get; private set; }

        protected override void OnStartRunning()
        {
            sceneSystem = World.GetExistingSystem<SceneSystem>();
            gameSceneEntity = GetSingletonEntity<SceneReference>();
            
            var factorysEntity = GetSingletonEntity<FactoriesComponentData>();
            var factorysData = EntityManager.GetComponentData<FactoriesComponentData>(factorysEntity);
            
            testEntity = factorysData.GetPrefab<TestEntitySharedConfig>();
        }

        protected override void OnUpdate()
        {
            if (IsInit) return;
            
            if (!sceneSystem.IsSceneLoaded(gameSceneEntity) 
                && EntityManager.HasComponent<ReadyPrefabTag>(testEntity))
            {
                LoadGameScene();

                IsInit = true;
            }
        }

        private void LoadGameScene()
        {
            var gameSceneEntityData = EntityManager.GetComponentData<SceneReference>(gameSceneEntity);

            sceneSystem.LoadSceneAsync(gameSceneEntityData.SceneGUID,
                new SceneSystem.LoadParameters {AutoLoad = false, Flags = SceneLoadFlags.LoadAsGOScene });
        }
    }
    
    
}