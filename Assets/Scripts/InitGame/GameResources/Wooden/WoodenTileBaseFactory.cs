using CraftCar.InitGame.GameResources.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CraftCar.InitGame.GameResources.Adressables
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