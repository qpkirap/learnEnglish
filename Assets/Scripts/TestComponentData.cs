using CraftCar.InitGame.GameResources.Adressables;
using Unity.Entities;

namespace CraftCar
{
    [GenerateAuthoringComponent]
    public class TestComponentData : IComponentData
    {
        public WoodenTileBaseFactory testBaseFactory;
    }
}