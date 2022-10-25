using Cysharp.Threading.Tasks;
using InitGame.GameResources.Base;
using Unity.Entities;
using UnityEngine;

namespace InitGame.GameResources.Adressables
{
    public abstract class WoodenTileBaseFactory : CreateEntityObjectsFactory
    {
        public async UniTask<GameObject> GetWoodTile()
        {
            var config = GetFactoryConfig;
            var key = config.woodPrefab;
            var go = await GetGameObjectPrefabAsync(key);
            return go;
        }

        public abstract WoodFactoryConfig GetFactoryConfig { get; }
    }
}