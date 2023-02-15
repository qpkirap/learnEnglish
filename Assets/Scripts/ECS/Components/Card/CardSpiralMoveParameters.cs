using Unity.Entities;
using Unity.Mathematics;

namespace CraftCar.ECS.Components
{
    public struct CardSpiralMoveParameters : IComponentData
    {
        public float accumulatedTime;
        public float kSpiral;
        public quaternion currentRotation;
        public int random;
    }
}