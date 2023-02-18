using System;
using System.Threading;
using Game.ECS_UI.Components;
using Game.ECS.Components;
using Cysharp.Threading.Tasks;
using Game.CustomPool;
using Unity.Entities;
using UnityEngine;

namespace Game.Config
{
    public abstract class CardMonoSharedComponent<TSharedConfig, TCardController, TConcreteShared> : CardMonoSharedComponent
        where TSharedConfig : EntitySharedComponent
        where TCardController : UICardController
        where TConcreteShared : struct, ICardInstance
    {
        [SerializeField] protected TSharedConfig config;
        
        protected static EntityManager manager => World.DefaultGameObjectInjectionWorld.EntityManager;

        public override async UniTask Init(Entity entity)
        {
            await config.Init(entity);
        }

        public override UICardController GetInstance(Entity entity, RectTransform parent = null)
        {
            return GetConcreteInstance(entity, parent);
        }

        public override Type GetSharedType => typeof(TConcreteShared);

        protected virtual TCardController GetPrefab(Entity entity)
        {
            config.CreateInstance(entity);

            var data = manager.GetSharedComponentData<TConcreteShared>(entity);

            return (TCardController) data.Instance;
        }
        
        private TCardController GetConcreteInstance(Entity entity, RectTransform parent = null)
        {
            var go = GetPrefab(entity).gameObject;
            
            var instance = PoolManager
                .Instance
                .SpawnObject(go, Vector3.zero, Quaternion.identity, parent)
                .GetComponent<TCardController>();

            var reference = manager.GetSharedComponentData<TConcreteShared>(entity);

            reference.Instance = instance;

            manager.SetSharedComponentData(entity, reference);
            
            return instance;
        }
    }

    public abstract class CardMonoSharedComponent : Component
    {
        public abstract UniTask Init(Entity entity);
        public abstract UICardController GetInstance(Entity entity, RectTransform parent = null);

        public abstract Type GetSharedType { get; }
    }
    

    public abstract class EntitySharedComponent <TComponent> : EntitySharedComponent
        where TComponent : struct, ISharedComponentData
    {
        protected static EntityManager manager => World.DefaultGameObjectInjectionWorld.EntityManager;
        protected static CancellationTokenSource tokenSource = new();
        protected TComponent instance;
        
        protected virtual async UniTask<TComponent> InitComponent()
        {
            return default;
        }

        public async override UniTask Init(Entity entity)
        {
            instance = await InitComponent();

            manager.AddSharedComponentData(entity, instance);

            manager.AddComponentData(entity, new ReadyPrefabTag());
        }

        public override void CreateInstance(Entity entity)
        {
            manager.AddComponentData(entity, new InstanceTag());
            
            manager.AddSharedComponentData(entity, instance);
        }
    }
    
    public abstract class EntitySharedComponent : Component
    {
        public abstract UniTask Init(Entity entity);

        public abstract void CreateInstance(Entity entity);
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