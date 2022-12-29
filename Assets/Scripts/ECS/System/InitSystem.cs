using CraftCar.ECS.Components.Tags;
using CraftCar.InitGame.ECS.Config;
using Unity.Entities;
using Unity.Scenes;

namespace CraftCar.ECS.System
{
    //[UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
    public partial class InitSystem : SystemBase
    {
        private Entity testEntity;
        private bool isLoadScene;

        protected override void OnStartRunning()
        {
            var factorysEntity = GetSingletonEntity<FactoriesComponentData>();
            var factorysData = EntityManager.GetComponentData<FactoriesComponentData>(factorysEntity);
            
            testEntity = factorysData.GetPrefab<TestEntitySharedConfig>();
        }

        protected override void OnUpdate()
        {
            if (!isLoadScene && EntityManager.HasComponent<ReadyPrefabTag>(testEntity))
            {
                isLoadScene = true;
                
                LoadGameScene();
            }
        }

        private void LoadGameScene()
        {
            var sceneSystem = World.GetExistingSystem<SceneSystem>();
            
            var gameSceneEntity = GetSingletonEntity<SceneReference>();
            
            var gameSceneEntityData = EntityManager.GetComponentData<SceneReference>(gameSceneEntity);

            sceneSystem.LoadSceneAsync(gameSceneEntityData.SceneGUID,
                new SceneSystem.LoadParameters {AutoLoad = false, Flags = SceneLoadFlags.LoadAsGOScene });
        }
    }
    
    
}