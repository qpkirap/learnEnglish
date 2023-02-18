using Unity.Entities;

namespace Game.ECS.Components
{
    public struct CardSpiralMoveParameters : IComponentData
    {
        public float AccumulatedTime;
        public float KSpiral;
        public int Random;
    }
}