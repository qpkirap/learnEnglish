using System;
using System.Collections.Generic;
using System.Linq;
using CraftCar.ECS_UI.Components;
using CraftCar.InitGame.GameResources.Base;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace CraftCar
{
    [GenerateAuthoringComponent]
    public class FactoriesCardData : IComponentData
    {
        public List<CardMonoSharedComponent> cardFabrics;

        public Dictionary<Type, CardMonoSharedComponent> cardFabricsDic;

        private static EntityManager manager => World.DefaultGameObjectInjectionWorld.EntityManager;

        public UICardController CreateCardInstance<T>(Entity entity) where T :CardMonoSharedComponent
        {
            var needFabrics = GetFabric<T>();

            if (needFabrics != null)
            {
                return needFabrics.GetInstance(entity);
            }

            return null;
        }

        public NativeArray<Entity> InitAllFabrics()
        {
            NativeArray<Entity> entities = new NativeArray<Entity>(cardFabrics.Count, Allocator.Temp);

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
                    
                    if(!cardFabricsDic.ContainsKey(typeCard)) cardFabricsDic.Add(typeCard, cardMono);
                }
            }

            return entities;
        }
        
      
        private T GetFabric<T>() where T : CardMonoSharedComponent
        {
            var type = typeof(T);
            
            if (cardFabricsDic.TryGetValue(type, out var factory))
            {
                return (T)factory;
            }
            
            Debug.LogError($"fabric not found {typeof(T).Name}");

            return null;
        }
        
    }
}