using CraftCar.ECS.Commander.Base;
using LearnEnglish.InitGame.GameResources;
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