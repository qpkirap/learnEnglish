using Unity.Collections;
using Unity.Entities;

namespace CraftCar.ECS.Components.SpawnData
{
    public struct DicElementData : IComponentData
    {
        public readonly int id;
        public readonly FixedString512Bytes en, ru, tr;
        
        public DicElementData
            (
            int id = -1,
            FixedString512Bytes en = default,
            FixedString512Bytes ru = default,
            FixedString512Bytes tr = default
            )
        {
            this.id = id;
            this.en = en;
            this.ru = ru;
            this.tr = tr;
        }
    }
}