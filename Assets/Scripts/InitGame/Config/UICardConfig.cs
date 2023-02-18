using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "UICardConfig", menuName = "Game/Configs/SharedComponent/UICardConfig")]
    public class UICardConfig : Component
    {
        [SerializeField] private AddressableGameObject go;

        public AddressableGameObject Go => go;
    }
}