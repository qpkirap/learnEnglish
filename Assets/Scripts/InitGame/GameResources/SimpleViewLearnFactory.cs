using System;
using CraftCar.ECS_UI.Components;
using CraftCar.InitGame.GameResources.Adressables;
using Cysharp.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace CraftCar.InitGame.GameResources
{
    [CreateAssetMenu(menuName = "LEARN/Factory/SimpleViewLearnFactory", fileName = "SimpleViewLearnFactory")]
    public class SimpleViewLearnFactory : ViewLearnBaseFactory
    {
        [SerializeField] private ViewLearnData viewLearnData;

        private Entity prefab;
        public override Entity GetPrefab => prefab;
        public override void Init()
        {
            InitEntityPrefab();
        }

        public override ViewLearnFactoryConfig GetFactoryConfig => viewLearnData.config;

        protected override async UniTask<Entity> InitEntityPrefab()
        {
            if (prefab != Entity.Null) return prefab;
            
            var go = await GetGameObjects();
            
            var uiCardController = go.GetComponent<UICardController>();
            
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            var entity = manager.CreateEntity(typeof(UICardControllerComponent));
            
            manager.AddSharedComponentData(entity,
                new UICardControllerComponent() { uiCardController = uiCardController });

            prefab = entity;

            IsLoadPrefab = true;
            
            return prefab;
        }
    }

    [Serializable]
    public class ViewLearnData
    {
        public ViewLearnFactoryConfig config;
    }
}