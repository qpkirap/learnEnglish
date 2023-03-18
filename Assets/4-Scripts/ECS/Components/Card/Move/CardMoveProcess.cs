using Unity.Entities;
using Unity.Mathematics;

namespace Game.ECS.Components
{
    public struct CardMoveProcess : IComponentData
    {
        public float2 NextPosition;
        public float2 NextScale;
        public float Width;
    }
}