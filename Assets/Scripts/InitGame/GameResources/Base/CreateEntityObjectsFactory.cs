using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CraftCar.InitGame.GameResources.Base
{
    public abstract class CreateEntityObjectsFactory : ScriptableObject
    {
        private static Dictionary<AssetReference, GameObject> loadedObjects = new Dictionary<AssetReference, GameObject>();

        protected Entity CreateEntity(Entity prefab)
        {
            var _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = _manager.Instantiate(prefab);
            return entity;
        }

        public static void ReleaseResources()
        {
            var loadReference = loadedObjects.Keys.ToArray();
            for (int i = 0; i < loadReference.Length; i++)
            {
                var key = loadReference[i];
                Destroy(loadedObjects[key]);
                key.ReleaseAsset();
            }
            loadedObjects.Clear();
        }

        public async UniTask<GameObject> GetGameObjectPrefabAsync(AssetReference key)
        {
            if (loadedObjects.TryGetValue(key, out var loadedGo))
            {
                return loadedGo;
            }

            if (!key.RuntimeKeyIsValid())
            {
                Debug.LogError($"ключ не валиден {key}");
                return null;
            }
            var handle = Addressables.LoadAssetAsync<GameObject>(key);
            var go = await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedObjects.Add(key, go);
            }
            else
            {
                Debug.LogError($"Не удалось загрузить adressable key={key}");
            }
            
            return null;
        }
    }   
}