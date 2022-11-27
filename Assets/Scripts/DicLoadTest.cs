using CraftCar.Config;
using Unity.Entities;

namespace CraftCar
{
    [GenerateAuthoringComponent]
    public class DicLoadTest : IComponentData
    {
        public DicJsonConfigAsset data;
        
    }
}