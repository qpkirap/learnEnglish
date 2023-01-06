using CraftCar.ECS_UI.Components;
using CraftCar.InitGame.GameResources.Base;
using Cysharp.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace CraftCar.InitGame.ECS.Config
{
    [CreateAssetMenu(fileName = "TestEntitySharedConfig", menuName = "Game/Configs/FabricsComponent/Shared/TestEntitySharedConfig")]
    public class TestEntitySharedConfig : EntitySharedComponent<UICardControllerComponent>
    {
        [SerializeField] private UICardControllerConfig cardControllerConfig;

        protected override async UniTask<UICardControllerComponent> InitComponent()
        {
            var go = await cardControllerConfig.GetConfig().Go.GetGameObjectPrefabAsync(tokenSource.Token);

            if (go != null) 
            {
                if (go.TryGetComponent<UICardController>(out UICardController uiCardController))
                {
                    var instance = Instantiate(uiCardController);
                        
                    return new(uiCardInstance: instance);
                }
            }
          
            return default;
        }
        
    }
}