using Unity.Entities;
using UnityEngine;

namespace CraftCar.ECS_UI.Components
{
    [GenerateAuthoringComponent]
    public class UICanvasAuthoring : IComponentData
    {
        public Transform root;
    }
}