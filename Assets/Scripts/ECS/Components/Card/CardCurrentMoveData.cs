using Unity.Entities;
using Unity.Mathematics;

namespace CraftCar.ECS.Components
{
    public struct CardCurrentMoveData : IComponentData
    {
        public float2 currentPosition;
        public float2 currentLocalScale;
    }
}