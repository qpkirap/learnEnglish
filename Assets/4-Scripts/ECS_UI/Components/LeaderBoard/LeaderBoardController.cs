using TMPro;
using Unity.Entities;

namespace Game.ECS_UI.Components
{
    [GenerateAuthoringComponent]
    public class LeaderBoardController : IComponentData
    {
        public TMP_Text currentClickPoint;
        public TMP_Text leaderText;
        public TMP_Text leaderPoint;
        public TMP_Text currentNick;
    }
}