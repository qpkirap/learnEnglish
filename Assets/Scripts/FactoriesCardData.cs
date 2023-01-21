using System;
using System.Collections.Generic;
using CraftCar.ECS_UI.Components;
using CraftCar.InitGame.GameResources.Base;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CraftCar
{
    [GenerateAuthoringComponent]
    public class FactoriesCardData : IComponentData
    {
        public List<CardMonoSharedComponent> cardFabrics;

        public Dictionary<Type, List<CardMonoSharedComponent>> cardFabricsDic;

        private static EntityManager manager => World.DefaultGameObjectInjectionWorld.EntityManager;

        public UICardController CreateCardInstance<T>(Entity entity, RectTransform parent = null) where T :CardMonoSharedComponent
        {
            var needFabrics = GetFabric<T>();

            if (needFabrics != null)
            {
                return needFabrics.GetInstance(entity, parent);
            }

            return null;
        }

        public NativeArray<Entity> InitAllFabrics()
        {
            NativeArray<Entity> entities = new NativeArray<Entity>(cardFabrics.Count, Allocator.Persistent);

            if (cardFabrics != null)
            {
                cardFabricsDic ??= new();

                for (var index = 0; index < cardFabrics.Count; index++)
                {
                    var cardMono = cardFabrics[index];
                    var typeCard = cardMono.GetType();
                    var entity = manager.CreateEntity(cardMono.GetSharedType);

                    cardMono.Init(entity);

                    entities[index] = entity;
                    
                    if(!cardFabricsDic.ContainsKey(typeCard)) cardFabricsDic.Add(typeCard, new List<CardMonoSharedComponent>(){cardMono});
                    else cardFabricsDic[typeCard].Add(cardMono);
                }
            }

            return entities;
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
        
    }
}