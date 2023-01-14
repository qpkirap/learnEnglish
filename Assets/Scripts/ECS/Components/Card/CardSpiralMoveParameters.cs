using Unity.Entities;

namespace CraftCar.ECS.Components
{
    public struct CardSpiralMoveParameters : IComponentData
    {
        public float accumulatedTime;
        public float kSpiral;
        public int random;
    }
}