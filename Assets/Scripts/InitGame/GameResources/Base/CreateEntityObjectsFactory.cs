using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace InitGame.GameResources.Base
{
    public abstract class CreateEntityObjectsFactory : ScriptableObject
    {
        private static Dictionary<AssetReference, GameObject> loadedObjects = new Dictionary<AssetReference, GameObject>();
        private static List<AssetReference> useAssets = new List<AssetReference>();

        protected Entity CreateEntity(Entity prefab)
        {
            var _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = _manager.Instantiate(prefab);
            return entity;
        }

        public async UniTask<GameObject> GetGameObjectPrefabAsync(AssetReference key)
        {
            if (loadedObjects.TryGetValue(key, out var loadedGo))
            {
                return loadedGo;
            }
            var handle = Addressables.LoadAssetAsync<GameObject>(key);
            var go = await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedObjects.Add(key, go);
                useAssets.Add(key);
            }
            else
            {
                Debug.LogError($"Не удалось загрузить adressable key={key}");
            }
            
            return null;
        }
    }   
}