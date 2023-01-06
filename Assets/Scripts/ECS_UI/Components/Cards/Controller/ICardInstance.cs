using Unity.Entities;

namespace CraftCar.ECS_UI.Components
{
    public interface ICardInstance : ISharedComponentData
    {
        public UICardController GetInstance { get; }
    }
}