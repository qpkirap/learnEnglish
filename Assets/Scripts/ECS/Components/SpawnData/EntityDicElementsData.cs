using Unity.Entities;
using Unity.Mathematics;
using Random = UnityEngine.Random;

namespace CraftCar.ECS.Components.SpawnData
{
    [GenerateAuthoringComponent]
    public struct EntityDicElementsData : IComponentData
    {
        public BlobAssetReference<DicElementsData> dicElementsData;

        public DicElementData GetRandomData()
        {
            if (dicElementsData == null) return default;

            return dicElementsData.Value.dataArray[Random.Range(0, dicElementsData.Value.dataArray.Length)];
        }
    }
}