using Unity.Entities;
using UnityEngine;

namespace InitGame.GameResources.Base
{
    public abstract class EntityShareComponentGameObjectsFactory : ScriptableObject
    {
        
        protected Entity CreateEntity<T>(T prefab) where T : IEntitySharedGameObjectConfig
        {
            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            var goEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab.monoBehaviour.gameObject, settings);

            return goEntity;
        }
    }
}