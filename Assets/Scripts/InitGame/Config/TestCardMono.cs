using CraftCar.ECS_UI.Components;
using CraftCar.InitGame.GameResources.Base;
using UnityEngine;

namespace CraftCar.InitGame.ECS.Config
{
    [CreateAssetMenu(fileName = "TestCardMono", menuName = "Game/TestCardMono")]
    public class TestCardMono : CardMonoSharedComponent<TestEntitySharedConfig, UICardController, UICardControllerComponent>
    {
    }
   
}