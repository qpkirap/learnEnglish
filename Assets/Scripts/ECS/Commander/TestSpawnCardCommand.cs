using CraftCar.ECS.Commander.Base;
using CraftCar.InitGame.GameResources.Base;
using Game.CustomPool;
using Unity.Entities;

namespace CraftCar.ECS.Commander
{
    public class TestSpawnCardCommand : ICommand, IComponentData
    {
        public EntitySharedComponent prefab;
        
        
        public void Execute(in Entity e)
        {
            //manager.AddComponent<CardLearnTag>(e);
            
            var pool = PoolManager.Instance;
            
            //pool.SpawnObject(prefab.Init())
        }
    }
}