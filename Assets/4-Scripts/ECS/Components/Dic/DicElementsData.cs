using Unity.Entities;

namespace Game.ECS.Components
{
    public struct DicElementsData
    {
        public BlobArray<DicElementData> DataArray;
    }
}