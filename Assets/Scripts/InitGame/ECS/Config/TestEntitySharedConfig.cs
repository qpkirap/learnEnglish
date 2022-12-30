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

        public override async UniTask<UICardControllerComponent> GetComponent(Entity entity)
        {
            if (entity != Entity.Null)
            {
                var go = await cardControllerConfig.GetConfig().Go.GetGameObjectPrefabAsync(tokenSource.Token);

                if (go != null)
                {
                    if (go.TryGetComponent<UICardController>(out UICardController uiCardController))
                    {
                        var test = Instantiate(uiCardController);
                        
                        return new(uiCardController: test);
                    }
                }
            }

            return default;
        }
    }
}