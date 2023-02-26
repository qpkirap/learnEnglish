using Unity.Entities;

namespace Game.ECS_UI.Components
{
    public interface IRegistrationPanelInstance : ISharedComponentData
    {
        public UIRegistrationPanel Instance { get; set; }
    }
}