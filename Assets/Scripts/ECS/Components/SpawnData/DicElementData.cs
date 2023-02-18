using Unity.Collections;
using Unity.Entities;

namespace Game.ECS.Components
{
    public struct DicElementData : IComponentData
    {
        public readonly int Id;
        public readonly FixedString512Bytes En, Ru, Tr;

        public DicElementData
        (
            int id = -1,
            FixedString512Bytes en = default,
            FixedString512Bytes ru = default,
            FixedString512Bytes tr = default
        )
        {
            this.Id = id;
            this.En = en;
            this.Ru = ru;
            this.Tr = tr;
        }
    }
}