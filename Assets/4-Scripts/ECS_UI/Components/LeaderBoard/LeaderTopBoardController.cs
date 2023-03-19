using TMPro;
using Unity.Entities;

namespace Game.ECS_UI.Components
{
    [GenerateAuthoringComponent]
    public class LeaderTopBoardController : IComponentData
    {
        public TMP_Text leaderText;
        public TMP_Text leaderPoint;
    }
}