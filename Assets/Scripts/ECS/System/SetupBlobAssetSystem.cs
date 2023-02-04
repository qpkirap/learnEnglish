using CraftCar;
using CraftCar.Config;
using CraftCar.ECS.Components.SpawnData;
using Game.ECS.System.Base;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Entities;

namespace Game.ECS.System
{
    public partial class SetupBlobAssetSystem : InitSystemBase
    {
        protected override void OnStartRunning()
        {
            var dataEntity = GetSingletonEntity<DicLoadTest>();
            var data = EntityManager.GetComponentData<DicLoadTest>(dataEntity);
            
            var loadText = JsonConvert.DeserializeObject<DicJsonConfig>(data.data.EngToRuDicData.ToString());
            
            using var blobBuilder = new BlobBuilder(Allocator.Temp);
            ref var dicElementsBlobAsset = ref blobBuilder.ConstructRoot<DicElementsData>();
            var elementArrayBlob = blobBuilder.Allocate(ref dicElementsBlobAsset.dataArray, loadText._dataArrays.Length);

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
            blobDicEntity.dicElementsData = blobBuilder.CreateBlobAssetReference<DicElementsData>(Allocator.Persistent);
            
            SetSingleton(blobDicEntity);
            
            EntityManager.DestroyEntity(dataEntity);
        }   


        protected override void OnUpdate()
        {
        }
    }
}