using System;
using Unity.Entities;

namespace CraftCar.ECS_UI.Components
{
    public struct UICardControllerComponent : IEquatable<UICardControllerComponent>, ICardInstance
    {
        public UICardController uiCardInstance;

        public UICardController Instance
        {
            get => uiCardInstance;
            set => uiCardInstance = value;
        }

        public UICardControllerComponent(UICardController uiCardInstance)
        {
            this.uiCardInstance = uiCardInstance;
        }

        public bool Equals(UICardControllerComponent other)
        {
            return Equals(uiCardInstance, other.uiCardInstance);
        }

        public override bool Equals(object obj)
        {
            return obj is UICardControllerComponent other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (uiCardInstance != null ? uiCardInstance.GetHashCode() : 0);
        }
    }
}