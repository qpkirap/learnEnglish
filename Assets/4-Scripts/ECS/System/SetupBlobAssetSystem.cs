using Game.Config;
using Game.ECS.Components;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Entities;

namespace Game.ECS.System
{
    public partial class SetupBlobAssetSystem : InitSystemBase
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnStartRunning()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer();

            var dataEntity = GetSingletonEntity<DicLoad>();
            var data = EntityManager.GetComponentData<DicLoad>(dataEntity);
            
            var loadText = JsonConvert.DeserializeObject<DicJsonConfig>(data.data.EngToRuDicData.ToString());
            
            using var blobBuilder = new BlobBuilder(Allocator.Temp);
            ref var dicElementsBlobAsset = ref blobBuilder.ConstructRoot<DicElementsData>();
            var elementArrayBlob = blobBuilder.Allocate(ref dicElementsBlobAsset.DataArray, loadText._dataArrays.Length);

            for (int i = 0; i < loadText._dataArrays.Length; i++)
            {
                elementArrayBlob[i] = 
                    new DicElementData(
                        loadText._dataArrays[i].id,
                        loadText._dataArrays[i].en,
                        loadText._dataArrays[i].ru,
                        loadText._dataArrays[i].tr
                    );
            }
            
            var blobDicEntity = GetSingleton<EntityDicElementsData>();
            blobDicEntity.DicElementsData = blobBuilder.CreateBlobAssetReference<DicElementsData>(Allocator.Persistent);
            
            SetSingleton(blobDicEntity);
            
            ecb.DestroyEntity(dataEntity);
        }   
        
        protected override void OnUpdate()
        {
        }
    }
}