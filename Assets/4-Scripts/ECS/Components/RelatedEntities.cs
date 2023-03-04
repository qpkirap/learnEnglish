using Unity.Entities;

namespace Game.ECS.System
{
    public struct RelatedEntities : IBufferElementData
    {
        public Entity entity;
    }
}