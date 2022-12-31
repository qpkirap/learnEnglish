using CraftCar.ECS.Commander.Base;
using CraftCar.ECS.Components.Tags;
using CraftCar.InitGame.GameResources.Base;
using Game.CustomPool;
using Unity.Entities;

namespace CraftCar.ECS.Commander
{
    public struct TestSpawnCardCommand : ICommand, IComponentData
    {
        private EntitySharedComponent prefab;
        
        public TestSpawnCardCommand(EntitySharedComponent prefab)
        {
            this.prefab = prefab;
        }
        
        private static EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        public void Execute(in Entity e)
        {
            manager.AddComponent<CardLearnTag>(e);
            
            var pool = PoolManager.Instance;
            
            //pool.SpawnObject(prefab.Init())
        }
    }
}