using Unity.Entities;
using Unity.Mathematics;

namespace CraftCar.ECS.Components
{
    public struct CardSpiralMoveData : IComponentData
    {
        public float2 nextPosition;
        public float2 nextScale;
    }
}