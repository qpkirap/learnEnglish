using System;
using System.Threading;
using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components;
using CraftCar.ECS.Components.Tags;
using Cysharp.Threading.Tasks;
using Game.CustomPool;
using Unity.Entities;
using UnityEngine;
using Transform = log4net.Util.Transform;

namespace CraftCar.InitGame.GameResources.Base
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

            return (TCardController) data.GetInstance;
        }
        
        private TCardController GetConcreteInstance(Entity entity, RectTransform parent = null)
        {
            var go = GetPrefab(entity).gameObject;
            
            return PoolManager.Instance.SpawnObject(go, Vector3.zero, Quaternion.identity, parent).GetComponent<TCardController>();
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

        private TComponent GetInstance()
        {
            return instance;
        }


        //TODO перед созданием сущности нужно создать архетип
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