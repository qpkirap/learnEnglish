using Game.Config;
using Unity.Entities;

namespace Game
{
    [GenerateAuthoringComponent]
    public class DicLoadTest : IComponentData
    {
        public DicJsonConfigAsset data;
    }
}