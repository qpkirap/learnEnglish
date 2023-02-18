using Unity.Entities;

namespace Game.ECS_UI.Components
{
    public interface ICardInstance : ISharedComponentData
    {
        public UICardController Instance { get; set; }
    }
}