using Game.ECS_UI.Components;
using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "UIRegistrationPanelFabric", menuName = "Game/Configs/Fabrics/FB_Registration/SimpleSharedFabricConfig")]
    public class UIRegistrationPanelFabric : PanelRegMonoSharedFabric<UIRegistrationPanelFabricConfig, UIRegistrationPanel, UIRegistrationPanelComponent>
    {
    }
}