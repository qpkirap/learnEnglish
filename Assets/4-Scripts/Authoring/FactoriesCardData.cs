using System;
using System.Collections.Generic;
using Game.ECS_UI.Components;
using Game.Config;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    [GenerateAuthoringComponent]
    public class FactoriesCardData : IComponentData
    {
        public List<CardMonoSharedComponent> cardFabrics;
        public List<PanelRegMonoSharedComponent> regFabrics;

        public Dictionary<Type, List<CardMonoSharedComponent>> cardFabricsDic;
        public Dictionary<Type, List<PanelRegMonoSharedComponent>> regFabricsDic;

        private static EntityManager manager => World.DefaultGameObjectInjectionWorld.EntityManager;

        public UICardController CreateCardInstance<T>(Entity entity, RectTransform parent = null) where T : CardMonoSharedComponent
        {
            var needFabrics = GetFabric<T>();

            if (needFabrics != null)
            {
                return needFabrics.GetInstance(entity, parent);
            }

            return null;
        }
        
        public UIRegistrationPanel CreateRegInstance<T>(Entity entity, RectTransform parent = null) where T : PanelRegMonoSharedComponent
        {
            var needFabrics = GetRegFabric<T>();

            if (needFabrics != null)
            {
                return needFabrics.GetInstance(entity, parent);
            }

            return null;
        }

        public void InitAllFabrics()
        {
            if (cardFabrics != null)
            {
                cardFabricsDic ??= new();

                for (var index = 0; index < cardFabrics.Count; index++)
                {
                    var cardMono = cardFabrics[index];
                    var typeCard = cardMono.GetType();
                    var entity = manager.CreateEntity(cardMono.GetSharedType);

                    manager.AddComponentData(entity, new Prefab());

                    cardMono.Init(entity);

                    if (!cardFabricsDic.ContainsKey(typeCard)) cardFabricsDic.Add(typeCard, new List<CardMonoSharedComponent>(){cardMono});
                    else cardFabricsDic[typeCard].Add(cardMono);
                }
            }
            
            if (regFabrics != null)
            {
                regFabricsDic ??= new();

                for (var index = 0; index < regFabrics.Count; index++)
                {
                    var panelMono = regFabrics[index];
                    var typeCard = panelMono.GetType();
                    var entity = manager.CreateEntity(panelMono.GetSharedType);

                    manager.AddComponentData(entity, new Prefab());

                    panelMono.Init(entity);

                    if (!regFabricsDic.ContainsKey(typeCard)) regFabricsDic.Add(typeCard, new List<PanelRegMonoSharedComponent>(){panelMono});
                    else regFabricsDic[typeCard].Add(panelMono);
                }
            }
        }

        private T GetFabric<T>() where T : CardMonoSharedComponent
        {
            var type = typeof(T);
            
            if (cardFabricsDic.TryGetValue(type, out var factory))
            {
                return (T)factory[Random.Range(0, factory.Count)];
            }
            
            Debug.LogError($"fabric not found {typeof(T).Name}");

            return null;
        }
        
        private T GetRegFabric<T>() where T : PanelRegMonoSharedComponent
        {
            var type = typeof(T);
            
            if (regFabricsDic.TryGetValue(type, out var factory))
            {
                return (T)factory[Random.Range(0, factory.Count)];
            }
            
            Debug.LogError($"fabric not found {typeof(T).Name}");

            return null;
        }
    }
}