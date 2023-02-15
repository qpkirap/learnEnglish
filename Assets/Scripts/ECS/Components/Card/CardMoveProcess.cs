using Unity.Entities;
using Unity.Mathematics;

namespace CraftCar.ECS.Components
{
    public struct CardMoveProcess : IComponentData
    {
        public quaternion nextLocalRotation;
        public float2 nextPosition;
        public float2 nextScale;
    }
}