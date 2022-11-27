using CraftCar.InitGame.GameResources.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CraftCar.InitGame.GameResources.Adressables
{
    public abstract class ViewLearnBaseFactory : CreateEntityObjectsFactory
    {
        protected async UniTask<GameObject> GetGameObjects()
        {
            var config = GetFactoryConfig;
            var key = config.uiCardPrefab;
            var go = await GetGameObjectPrefabAsync(key);
            return go;
        }

        public abstract ViewLearnFactoryConfig GetFactoryConfig { get; }
    }
}