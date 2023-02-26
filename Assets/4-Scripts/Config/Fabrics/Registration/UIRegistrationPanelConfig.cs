using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "UIRegistrationPanelConfig", menuName = "Game/Configs/Fabrics/FB_Registration/Components/UIRegistrationPanelConfig")]
    public class UIRegistrationPanelConfig : Component
    {
        [SerializeField] private AddressableGameObject go;

        public AddressableGameObject Go => go;
    }
}