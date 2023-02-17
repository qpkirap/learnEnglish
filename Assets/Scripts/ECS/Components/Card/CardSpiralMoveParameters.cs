using Unity.Entities;

namespace CraftCar.ECS.Components
{
    public struct CardSpiralMoveParameters : IComponentData
    {
        public float AccumulatedTime;
        public float KSpiral;
        public int Random;
    }
}