using JohanPolosn.UnityInjector;
using Unity.Entities;
using UnityEngine;

namespace Game.ECS_UI.Components
{
    //[GenerateAuthoringComponent]
    public class UICanvasController : MonoBehaviour
    {
        public RectTransform Root;
        public RectTransform CardRoot;
        public LeaderBoardController LeaderBoard;
        public LeaderCanvas LeaderCanvas;
        public AdsCanvas.AdsCanvas adsCanvas;
        public UIRegistrationPanel registrationPanel;

        public Entity entity { get; private set; }
        
        private static EntityManager manager;

        private void OnEnable()
        {
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;

            entity = manager.CreateEntity();

            manager.AddComponentData(entity, new UICanvasControllerTag());
            
            DI.Add(this);
        }
    }

    public struct UICanvasControllerTag : IComponentData
    {
    }
}