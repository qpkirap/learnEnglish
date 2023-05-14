using Unity.Entities;
using UnityEngine;

namespace Game.ECS_UI.Components
{
    [GenerateAuthoringComponent]
    public class UICanvasController : IComponentData
    {
        public RectTransform Root;
        public RectTransform CardRoot;
        public LeaderBoardController LeaderBoard;
        public LeaderCanvas LeaderCanvas;
        public AdsCanvas.AdsCanvas adsCanvas;
        public UIRegistrationPanel registrationPanel;
    }
}