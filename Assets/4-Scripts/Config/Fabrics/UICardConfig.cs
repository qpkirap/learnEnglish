using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "UICardConfig", menuName = "Game/Configs/Fabrics/Simple/Components/UICardConfig")]
    public class UICardConfig : Component
    {
        [SerializeField] private AddressableGameObject go;

        public AddressableGameObject Go => go;
    }
}