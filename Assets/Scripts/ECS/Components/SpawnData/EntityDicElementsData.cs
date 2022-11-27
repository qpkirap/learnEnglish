using Unity.Entities;

namespace CraftCar.ECS.Components.SpawnData
{
    [GenerateAuthoringComponent]
    public struct EntityDicElementsData : IComponentData
    {
        public BlobAssetReference<DicElementsData> dicElementsData;
    }
}