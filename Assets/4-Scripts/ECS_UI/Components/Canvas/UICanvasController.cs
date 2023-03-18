using Unity.Entities;
using UnityEngine;

namespace Game.ECS_UI.Components
{
    [GenerateAuthoringComponent]
    public class UICanvasController : IComponentData
    {
        public RectTransform Root;
        public RectTransform CardRoot;
    }
}