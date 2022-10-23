using InitGame.GameResources.Base;
using Unity.Entities;

namespace InitGame.GameResources.Adressables
{
    public abstract class AdressablesSharedGOFactory : EntityShareComponentGameObjectsFactory
    {
        public Entity GetTest()
        {
            var config = GetFactoryConfig;
            var entity = CreateEntity(config.TestControllerPrefab);
            var data = config.TestControllerPrefab.CreateSharedComponentData(ref entity);
            return entity;
        }
        
        public abstract AdressablesSharedGOFactoryConfig GetFactoryConfig { get; }
    }
}