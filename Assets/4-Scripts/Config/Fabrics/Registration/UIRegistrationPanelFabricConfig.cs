using Cysharp.Threading.Tasks;
using Game.ECS_UI.Components;
using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "UIRegistrationPanelFabricConfig", menuName = "Game/Configs/Fabrics/FB_Registration/UIRegistrationPanelFabricConfig")]

    public class UIRegistrationPanelFabricConfig : MonoSharedFabricConfig<UIRegistrationPanelComponent>
    {
        [SerializeField] private UIRegistrationPanelComponentConfig config; 
        
        protected override async UniTask<UIRegistrationPanelComponent> InitComponent()
        {
            var go = await config.GetConfig().Go.GetGameObjectPrefabAsync(tokenSource.Token);

            if (go != null) 
            {
                if (go.TryGetComponent<UIRegistrationPanel>(out UIRegistrationPanel panel))
                {
                    return new(registrationPanel: panel);
                }
            }
          
            return default;
        }
    }
}