using CraftCar.ECS.Components.Tags;
using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;

namespace CraftCar.ECS.System
{
    [AlwaysUpdateSystem]
    //[UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
    public partial class InitSystem : SystemBase
    {
        private SceneSystem sceneSystem;
        
        private Entity gameSceneEntity;

        private NativeArray<Entity> fabricsCard;

        public bool IsInit { get; private set; }

        protected override void OnStartRunning()
        {
            sceneSystem = World.GetExistingSystem<SceneSystem>();
            gameSceneEntity = GetSingletonEntity<SceneReference>();
            
            var factorysEntity = GetSingletonEntity<FactoriesCardData>();
            var factorysData = EntityManager.GetComponentData<FactoriesCardData>(factorysEntity);

            fabricsCard = factorysData.InitAllFabrics();
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
            if(fabricsCard == null || !fabricsCard.IsCreated) return false;
            
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