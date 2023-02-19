using Game.ECS_UI.Components;
using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "SimpleCardFabric", menuName = "Game/Configs/Fabrics/Simple/SimpleCardFabric")]
    public class SimpleCardFabric : CardMonoSharedFabric<SimpleSharedFabricConfig, UICardController, UICardControllerComponent>
    {
    }
}