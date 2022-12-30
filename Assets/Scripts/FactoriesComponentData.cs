using System;
using System.Collections.Generic;
using System.Linq;
using CraftCar.InitGame.GameResources.Base;
using Cysharp.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace CraftCar
{
    [GenerateAuthoringComponent]
    public class FactoriesComponentData : IComponentData
    {
        public List<EntitySharedComponent> sharedFabrics;

        public Dictionary<Type, EntitySharedComponent> sharedFabricsDic;
        public Dictionary<Type, Entity> entitiesPrefabs;
        
        private static EntityManager manager => World.DefaultGameObjectInjectionWorld.EntityManager;
        
        public Entity GetPrefab<T>() where T : EntitySharedComponent
        {
            entitiesPrefabs ??= new();

            if (entitiesPrefabs.TryGetValue(typeof(T), out var entity))
            {
                return entity;
            }
            else
            {
                var entityNew= manager.CreateEntity(typeof(T));

                manager.AddComponent<Prefab>(entityNew);

                CreatePrefab<T>(entityNew);

                return entityNew;
            }
        }

        private async UniTask CreatePrefab<T>(Entity entity) where T : EntitySharedComponent
        {
            var needFabric = GetFabric<T>();

            if (needFabric != null)
            {
                if (entitiesPrefabs.ContainsKey(typeof(T)))
                {
                    entitiesPrefabs[typeof(T)] = entity;
                }
                else entitiesPrefabs.Add(typeof(T), entity);
                
                await needFabric.Create(entity);
            }
            else Debug.LogError($"{this.GetType().Name} CreatePrefab fabric not found");
        }
        
        
        private T GetFabric<T>() where T : EntitySharedComponent
        {
            sharedFabricsDic ??= new();
            
            if (!sharedFabricsDic.Any() && sharedFabrics != null) InitDictionary();
            
            var type = typeof(T);
            
            if (sharedFabricsDic.TryGetValue(type, out var factory))
            {
                return (T)factory;
            }
            
            Debug.LogError($"fabric not found {typeof(T).Name}");

            return null;
        }
        
        private void InitDictionary()
        {
            if (sharedFabrics != null && !sharedFabricsDic.Any())
            {
                for (int i = 0; i < sharedFabrics.Count; i++)
                {
                    if (sharedFabrics[i] != null)
                    {
                        var type = sharedFabrics[i].GetType();
                        
                        if (!sharedFabricsDic.ContainsKey(type))
                        {
                            sharedFabricsDic.Add(type, sharedFabrics[i]);
                        }
                        
                        else Debug.LogError($"multi fabrics {sharedFabrics[i].name}");
                    }
                }
            }
        }
    }
}