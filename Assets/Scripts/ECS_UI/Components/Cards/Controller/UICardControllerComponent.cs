using System;
using Unity.Entities;

namespace CraftCar.ECS_UI.Components
{
    public struct UICardControllerComponent : ISharedComponentData, IEquatable<UICardControllerComponent>
    {
        public UICardController uiCardController;

        public bool Equals(UICardControllerComponent other)
        {
            return Equals(uiCardController, other.uiCardController);
        }

        public override bool Equals(object obj)
        {
            return obj is UICardControllerComponent other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (uiCardController != null ? uiCardController.GetHashCode() : 0);
        }
    }
}