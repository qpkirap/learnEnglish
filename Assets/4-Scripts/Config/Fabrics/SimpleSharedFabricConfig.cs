using Game.ECS_UI.Components;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "SimpleSharedFabricConfig", menuName = "Game/Configs/Fabrics/Simple/SimpleSharedFabricConfig")]
    public class SimpleSharedFabricConfig : MonoSharedFabricConfig<UICardControllerComponent>
    {
        [FormerlySerializedAs("cardControllerConfig")] [SerializeField] private UICardControllerComponentConfig cardControllerComponentConfig;

        protected override async UniTask<UICardControllerComponent> InitComponent()
        {
            var go = await cardControllerComponentConfig.GetConfig().Go.GetGameObjectPrefabAsync(tokenSource.Token);

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