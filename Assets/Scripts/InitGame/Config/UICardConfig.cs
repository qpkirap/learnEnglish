using LearnEnglish.InitGame.GameResources;
using UnityEngine;
using Component = LearnEnglish.InitGame.GameResources.Component;

namespace CraftCar.InitGame.ECS.Config
{
    [CreateAssetMenu(fileName = "UICardConfig", menuName = "Game/Configs/SharedComponent/UICardConfig")]
    public class UICardConfig : Component
    {
        [SerializeField] private AddressableGameObject go;

        public AddressableGameObject Go => go;
    }
}