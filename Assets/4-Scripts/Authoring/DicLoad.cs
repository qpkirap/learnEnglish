using Game.Config;
using Unity.Entities;

namespace Game
{
    [GenerateAuthoringComponent]
    public class DicLoad : IComponentData
    {
        public DicJsonConfigAsset data;
    }
}