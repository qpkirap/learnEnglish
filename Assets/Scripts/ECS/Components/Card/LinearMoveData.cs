using Unity.Entities;

namespace Game.ECS.Components
{
    public struct LinearMoveData : IComponentData
    {
        public float AccumulatedTime;
        public float InitDistanceToTarget;
        public bool IsPreLastMove;
    }
}