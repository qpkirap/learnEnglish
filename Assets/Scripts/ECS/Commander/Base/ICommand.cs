using Unity.Entities;

namespace CraftCar.ECS.Commander.Base
{
    public interface ICommand
    {
        void Execute(in Entity e);
    }
}