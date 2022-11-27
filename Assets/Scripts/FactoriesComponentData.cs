using System;
using System.Collections.Generic;
using System.Linq;
using CraftCar.InitGame.GameResources.Base;
using Unity.Entities;
using UnityEngine;

namespace CraftCar
{
    [GenerateAuthoringComponent]
    public class FactoriesComponentData : IComponentData
    {
        public List<CreateEntityObjectsFactory> UiCardFabrics;
        
        public Dictionary<Type, CreateEntityObjectsFactory> uiCardsDictionary;
        
        private void InitDictionary(List<CreateEntityObjectsFactory> listFabrics, ref Dictionary<Type, CreateEntityObjectsFactory> dictionary)
        {
            if (listFabrics != null && !dictionary.Any())
            {
                for (int i = 0; i < listFabrics.Count; i++)
                {
                    if (listFabrics[i] != null)
                    {
                        var type = listFabrics[i].GetType();
                        if (!dictionary.ContainsKey(type))
                        {
                            dictionary.Add(type, listFabrics[i]);
                        }
                        else Debug.LogError($"multi fabrics {listFabrics[i].name}");
                    }
                }
            }
        }
        
        public void InitTriggers()
        {
            uiCardsDictionary ??= new();
            InitDictionary(UiCardFabrics, ref uiCardsDictionary);
        }
        
        public T GetFabric<T>() where T : CreateEntityObjectsFactory
        {
            InitTriggers();
            var type = typeof(T);
            if (uiCardsDictionary.TryGetValue(type, out var factory))
            {
                return (T)factory;
            }
            Debug.LogError($"fabric not found {typeof(T).Name}");

            return null;
        }
    }
}