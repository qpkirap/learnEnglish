using Unity.Entities;
using Unity.Mathematics;

namespace Game.ECS.Components
{
    public struct TargetMoveData : IComponentData
    {
        public float2 TargetMove;
    }
}