using Game.ECS_UI.Components;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Config
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
                    return new(uiCardInstance: uiCardController);
                }
            }
          
            return default;
        }
    }
}