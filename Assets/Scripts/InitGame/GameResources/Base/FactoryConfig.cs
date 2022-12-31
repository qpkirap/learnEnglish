using System.Threading;
using CraftCar.ECS.Components.Tags;
using Cysharp.Threading.Tasks;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace CraftCar.InitGame.GameResources.Base
{
    public abstract class EntitySharedComponent <TComponent> : EntitySharedComponent
        where TComponent : struct, ISharedComponentData
    {
        protected static EntityManager manager => World.DefaultGameObjectInjectionWorld.EntityManager;
        protected static CancellationTokenSource tokenSource = new();
        
        public virtual async UniTask<TComponent> GetComponent(Entity entity)
        {
            return default;
        }
        

        //TODO перед созданием сущности нужно создать архетип
        public async override UniTask Init(Entity entity)
        {
            var component = await GetComponent(entity);

            manager.AddSharedComponentData(entity, component);

            manager.AddComponentData(entity, new ReadyPrefabTag());
        }
    }
    

    public abstract class EntitySharedComponent : Component
    {
        public abstract UniTask Init(Entity entity);
    } 
    
    public abstract class FactoryConfig<TConfig> : FactoryConfig
        where TConfig : Component
    {
        [SerializeField] protected TConfig prefab;
        
        public virtual TConfig GetConfig()
        {
            return prefab;
        }

        public override T GetComponent<T>()
        {
            if (typeof(T) == typeof(TConfig)) return prefab as T;
            
            return null;
        }
    }

    public abstract class FactoryConfig : ScriptableObject
    {
        public abstract T GetComponent<T>() where T : Component;
    }
    
    public abstract class Component : ScriptableObject
    {
    }
}