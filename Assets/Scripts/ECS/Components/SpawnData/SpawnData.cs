using Unity.Entities;
using Unity.Mathematics;

namespace Game.ECS.Components
{
    public struct SpawnData: IComponentData
    {
        public Entity prefabEntity;
        public float3 spawnPosition;
    }
}