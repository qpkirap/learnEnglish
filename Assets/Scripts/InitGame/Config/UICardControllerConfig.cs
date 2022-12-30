using CraftCar.InitGame.GameResources.Base;
using UnityEngine;

namespace CraftCar.InitGame.ECS.Config
{
    [CreateAssetMenu(fileName = "UICardControllerConfig", menuName = "Game/Configs/SharedComponent/UICardControllerConfig")]
    public class UICardControllerConfig : FactoryConfig<UICardConfig>
    {
    }
}