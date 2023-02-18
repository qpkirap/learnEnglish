using Unity.Entities;
using Random = UnityEngine.Random;

namespace Game.ECS.Components
{
    [GenerateAuthoringComponent]
    public struct EntityDicElementsData : IComponentData
    {
        public BlobAssetReference<DicElementsData> DicElementsData;

        public DicElementData GetRandomData()
        {
            return DicElementsData.Value.DataArray[Random.Range(0, DicElementsData.Value.DataArray.Length)];
        }
    }
}