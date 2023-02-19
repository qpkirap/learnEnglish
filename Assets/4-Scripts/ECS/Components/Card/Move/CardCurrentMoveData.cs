using Unity.Entities;
using Unity.Mathematics;

namespace Game.ECS.Components
{
    public struct CardCurrentMoveData : IComponentData
    {
        public float2 CurrentPosition;
        public float2 CurrentLocalScale;
    }
}