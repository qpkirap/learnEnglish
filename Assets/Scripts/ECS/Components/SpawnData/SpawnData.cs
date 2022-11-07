using Unity.Entities;
using Unity.Mathematics;

namespace CraftCar.ECS.Components.SpawnData
{
    public struct SpawnData: IComponentData
    {
        public Entity prefabEntity;
        public float3 spawnPosition;
    }
}