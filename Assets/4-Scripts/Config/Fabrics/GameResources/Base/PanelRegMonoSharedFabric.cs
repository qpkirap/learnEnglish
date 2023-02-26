using System;
using Cysharp.Threading.Tasks;
using Game.CustomPool;
using Game.ECS_UI.Components;
using Unity.Entities;
using UnityEngine;

namespace Game.Config
{
    public abstract class PanelRegMonoSharedFabric<TSharedConfig, TPanel, TConcreteShared> : PanelRegMonoSharedComponent
        where TSharedConfig : EntitySharedComponent
        where TPanel : UIRegistrationPanel
        where TConcreteShared : struct, IRegistrationPanelInstance
    {
        [SerializeField] protected TSharedConfig config;
        
        protected static EntityManager manager => World.DefaultGameObjectInjectionWorld.EntityManager;

        public override async UniTask Init(Entity entity)
        {
            await config.Init(entity);
        }

        public override UIRegistrationPanel GetInstance(Entity entity, RectTransform parent = null)
        {
            return GetConcreteInstance(entity, parent);
        }

        public override Type GetSharedType => typeof(TConcreteShared);

        protected virtual TPanel GetPrefab(Entity entity)
        {
            config.CreateInstance(entity);

            var data = manager.GetSharedComponentData<TConcreteShared>(entity);

            return (TPanel) data.Instance;
        }
        
        private TPanel GetConcreteInstance(Entity entity, RectTransform parent = null)
        {
            var go = GetPrefab(entity).gameObject;
            
            var instance = PoolManager
                .Instance
                .SpawnObject(go, Vector3.zero, Quaternion.identity, parent)
                .GetComponent<TPanel>();

            var reference = manager.GetSharedComponentData<TConcreteShared>(entity);

            reference.Instance = instance;

            manager.SetSharedComponentData(entity, reference);
            
            return instance;
        }
    }
}