using CraftCar.InitGame.GameResources.Base;
using UnityEngine;
using Component = CraftCar.InitGame.GameResources.Base.Component;

namespace CraftCar.InitGame.ECS.Config
{
    [CreateAssetMenu(fileName = "UICardConfig", menuName = "Game/Configs/SharedComponent/UICardConfig")]
    public class UICardConfig : Component
    {
        [SerializeField] private AddressableGameObject go;

        public AddressableGameObject Go => go;
    }
}