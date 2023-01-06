
using Unity.Entities;
using UnityEngine;

namespace CraftCar.ECS_UI.Components
{
    [GenerateAuthoringComponent]
    public class UICanvasController : IComponentData
    {
        public RectTransform root;
    }
}