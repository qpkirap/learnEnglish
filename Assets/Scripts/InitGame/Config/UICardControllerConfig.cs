using LearnEnglish.InitGame.GameResources;
using UnityEngine;

namespace CraftCar.InitGame.ECS.Config
{
    [CreateAssetMenu(fileName = "UICardControllerConfig", menuName = "Game/Configs/SharedComponent/UICardControllerConfig")]
    public class UICardControllerConfig : FactoryConfig<UICardConfig>
    {
    }
}