using Unity.Entities;
using Unity.Mathematics;

namespace CraftCar.ECS.Components
{
    public struct TargetMoveData : IComponentData
    {
        public float2 TargetMove;
    }
}