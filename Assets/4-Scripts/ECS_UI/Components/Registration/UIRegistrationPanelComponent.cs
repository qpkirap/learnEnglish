using System;

namespace Game.ECS_UI.Components
{
    public struct UIRegistrationPanelComponent : IEquatable<UIRegistrationPanelComponent>, IRegistrationPanelInstance
    {
        public UIRegistrationPanel registrationPanel;

        public UIRegistrationPanel Instance
        {
            get => registrationPanel;
            set => registrationPanel = value;
        }

        public UIRegistrationPanelComponent(UIRegistrationPanel registrationPanel)
        {
            this.registrationPanel = registrationPanel;
        }
        
        public bool Equals(UIRegistrationPanelComponent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(registrationPanel, other.registrationPanel);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UIRegistrationPanelComponent)obj);
        }

        public override int GetHashCode()
        {
            return (registrationPanel != null ? registrationPanel.GetHashCode() : 0);
        }
    }
}