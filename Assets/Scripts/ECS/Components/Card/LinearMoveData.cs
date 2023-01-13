using Unity.Entities;

namespace CraftCar.ECS.Components
{
    public struct LinearMoveData : IComponentData
    {
        public float accumulatedTime;
        public float initDistanceToTarget;
    }
}