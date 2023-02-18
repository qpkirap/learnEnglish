using Game.ECS_UI.Components;
using Game.Config;
using UnityEngine;

namespace Game.Config
{
    [CreateAssetMenu(fileName = "TestCardMono", menuName = "Game/TestCardMono")]
    public class TestCardMono : CardMonoSharedComponent<TestEntitySharedConfig, UICardController, UICardControllerComponent>
    {
    }
   
}